using Cysharp.Threading.Tasks;
using FHH.Logic.Models;
using FHH.UI;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FHH.Logic
{
    internal sealed class LayerDownloader : ILayerDownloader
    {
        private readonly IAuthenticationService _auth;
        private readonly IHttpClient _http;
        private readonly AppConfig _appConfig;

        private readonly SemaphoreSlim _downloadSemaphore = new SemaphoreSlim(8, 8);

        public string DownloadRoot { get; }

        [Serializable]
        private sealed class LayerManifest
        {
            [JsonProperty("id")] public string Id;
            [JsonProperty("name")] public string Name;
            [JsonProperty("href")] public string Href;
            [JsonProperty("type")] public string Type; // "TILES3D" | "IMAGERY" | "TERRAIN"
            [JsonProperty("downloadedAtUtc")] public DateTime DownloadedAtUtc;
            [JsonProperty("totalBytes")] public long TotalBytes;
            [JsonProperty("note")] public string Note;
            [JsonProperty("imagery")] public ImageryInfo Imagery;
            [JsonProperty("access")] public string Access; // "PUBLIC" | "RESTRICTED"
        }

        [Serializable]
        private sealed class ImageryInfo
        {
            [JsonProperty("template")] public string Template;
            [JsonProperty("minZ")] public int MinZ;
            [JsonProperty("maxZ")] public int MaxZ;
            [JsonProperty("bounds")] public double[] Bounds;
            [JsonProperty("countTiles")] public long CountTiles;
            [JsonProperty("ext")] public string Ext;
        }

        public LayerDownloader(IAuthenticationService auth, IHttpClient http, AppConfig appConfig)
        {
            _auth = auth;
            _http = http;
            _appConfig = appConfig;

            DownloadRoot = Path.Combine(UnityEngine.Application.persistentDataPath, "VRVisData");
            try
            {
                if (!Directory.Exists(DownloadRoot)) Directory.CreateDirectory(DownloadRoot);
            }
            catch (Exception e)
            {
                ULog.Warning($"Failed to ensure download folder: {DownloadRoot}. {e.Message}");
            }
        }

        public async UniTask RefreshDownloadedStatusAsync(IEnumerable<BaseLayerCombined> layers)
        {
            foreach (var layer in layers)
            {
                var root = GetLayerRootFolder(layer.Id);
                var manifestPath = Path.Combine(root, "manifest.json");
                if (Directory.Exists(root) && File.Exists(manifestPath))
                {
                    layer.IsDownloaded = true;
                    try
                    {
                        var mf = JsonConvert.DeserializeObject<LayerManifest>(File.ReadAllText(manifestPath));
                        if (mf != null && mf.TotalBytes > 0)
                        {
                            layer.SizeGb = mf.TotalBytes / (1024f * 1024f * 1024f);
                            layer.DownloadedAt = mf.DownloadedAtUtc;
                            if (!string.IsNullOrWhiteSpace(mf.Name)) layer.Name = mf.Name;
                            if (string.IsNullOrWhiteSpace(layer.Id) && !string.IsNullOrWhiteSpace(mf.Id))
                                layer.Id = mf.Id;
                        }
                        else
                        {
                            var bytes = SumFolderBytes(root);
                            layer.SizeGb = bytes / (1024f * 1024f * 1024f);
                            layer.DownloadedAt = Directory.GetCreationTimeUtc(root);
                        }
                    }
                    catch
                    {
                        var bytes = SumFolderBytes(root);
                        layer.SizeGb = bytes / (1024f * 1024f * 1024f);
                        layer.DownloadedAt = Directory.GetCreationTimeUtc(root);
                    }
                }
                else
                {
                    layer.IsDownloaded = false;
                    layer.SizeGb = null;
                    layer.DownloadedAt = null;
                }
                await UniTask.Yield();
            }
        }

        public async UniTask<bool> DownloadLayerAsync(BaseLayerCombined layer, CancellationToken ct = default)
        {
            if (layer == null || string.IsNullOrWhiteSpace(layer.Id) || string.IsNullOrWhiteSpace(layer.Href))
            {
                ULog.Warning("Invalid layer for download.");
                return false;
            }

            switch (layer.Type)
            {
                case BaseLayerType.Imagery:
                    ULog.Warning("Use DownloadImageryAsync(...) overload for imagery.");
                    return false;
                case BaseLayerType.Terrain:
                case BaseLayerType.Tiles3D:
                    return await Download3DTilesAsync(layer, ct);
                default:
                    ULog.Warning($"Unsupported layer type: {layer.Type}");
                    return false;
            }
        }


        public async UniTask<bool> DownloadImageryAsync(
            BaseLayerCombined layer,
            double minLon, double minLat, double maxLon, double maxLat,
            int minZoom, int maxZoom,
            CancellationToken ct = default)
        {
            if (layer == null || string.IsNullOrWhiteSpace(layer.Id) || string.IsNullOrWhiteSpace(layer.Href))
            {
                ULog.Warning("Invalid imagery layer for download.");
                return false;
            }
            if (layer.Type != BaseLayerType.Imagery)
            {
                ULog.Warning("DownloadImageryAsync called for non-imagery layer.");
                return false;
            }

            //var headers = BuildAuthHeaders();
            var headers = BuildResourceHeaders(layer.Href);
            var headHeaders = headers; // reuse for HEADs
            var root = GetLayerRootFolder(layer.Id);
            EnsureDirectory(root);
            var manifestPath = Path.Combine(root, "manifest.json");
            var templateTxt = Path.Combine(root, "template.txt");
            var chosenExt = InferImageExtensionFromTemplateOrUrl(layer.Href) ?? "png";
            try
            {
                //AtomicWrite(templateTxt, System.Text.Encoding.UTF8.GetBytes(layer.Href));
                await AtomicWriteAsync(templateTxt, Encoding.UTF8.GetBytes(layer.Href), ct);
            } 
            catch { }

            var tiles = EnumerateTiles(minLon, minLat, maxLon, maxLat, minZoom, maxZoom).ToList();
            long totalBytes = 0;

            long estimatedBytes = await EstimateTilesContentLengthAsync(layer.Href, tiles, headers, ct);
            if (!HasSufficientDiskSpace(root, estimatedBytes))
            {
                ULog.Warning($"Low disk space: estimated {BytesToHuman(estimatedBytes)} required. Aborting before download.");
                ServiceLocator.GetService<UIManager>().ShowNotification("LowDiskSpace");
                return false;
            }

            foreach (var t in tiles)
            {
                ct.ThrowIfCancellationRequested();
                var url = ResolveTemplate(layer.Href, t.Z, t.X, t.Y);
                var localPath = Path.Combine(root, $"{t.Z}", $"{t.X}", $"{t.Y}.{chosenExt}");
                if (HasHealthyFile(localPath)) continue;

                try { await _downloadSemaphore.WaitAsync(ct); } catch (OperationCanceledException) { break; }

                UniTask.Void(async () =>
                {
                    try
                    {
                        var resp = await _http.GetBytesAsync(url, headers, ct);
                        if (!resp.IsSuccess && (resp.StatusCode == 401 || resp.StatusCode == 403) && headers.ContainsKey("Authorization"))
                        {
                            var noAuth = new Dictionary<string, string>(headers);
                            noAuth.Remove("Authorization");
                            resp = await _http.GetBytesAsync(url, noAuth, ct);
                        }
                        if (resp.IsSuccess && resp.Bytes != null && resp.Bytes.Length > 0)
                        {
                            if (!HasSufficientDiskSpaceHardStop(localPath))
                            {
                                ServiceLocator.GetService<UIManager>().ShowNotification("InsufficientDiskSpace");
                                throw new IOException("Insufficient disk space while writing.");
                            }
                            //AtomicWrite(localPath, resp.Bytes);
                            await AtomicWriteAsync(localPath, resp.Bytes, ct);
                            Interlocked.Add(ref totalBytes, resp.Bytes.LongLength);
                        }
                        else
                        {
                            ULog.Warning($"Failed tile z{t.Z}/x{t.X}/y{t.Y}. Status: {resp.StatusCode}. Error: {resp.ErrorMessage}");
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex) { ULog.Warning($"Tile download failed z{t.Z}/x{t.X}/y{t.Y}: {ex.Message}"); }
                    finally { _downloadSemaphore.Release(); }
                });

                await UniTask.Yield();
            }

            // Wait for all permits to return
            while (_downloadSemaphore.CurrentCount < 8)
            {
                ct.ThrowIfCancellationRequested();
                await UniTask.Delay(50, cancellationToken: ct);
            }

            var mf = new LayerManifest
            {
                Id = layer.Id,
                Name = layer.Name,
                Href = layer.Href,
                Type = "IMAGERY",
                DownloadedAtUtc = DateTime.UtcNow,
                TotalBytes = totalBytes,
                Imagery = new ImageryInfo
                {
                    Template = layer.Href,
                    MinZ = minZoom,
                    MaxZ = maxZoom,
                    Bounds = new[] { minLon, minLat, maxLon, maxLat },
                    CountTiles = tiles.LongCount(),
                    Ext = chosenExt
                },
                Note = "Imagery offline cache",
                Access = layer.IsRestricted ? "RESTRICTED" : "PUBLIC"
            };

            try
            {
                //AtomicWrite(manifestPath, System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mf, Formatting.Indented)));
                await AtomicWriteAsync(manifestPath,System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mf, Formatting.Indented)), ct);
            } 
            catch { }

            layer.IsDownloaded = true;
            return true;
        }

        public async UniTask<bool> DeleteLayerAsync(BaseLayerCombined layer)
        {
            if (layer == null || string.IsNullOrWhiteSpace(layer.Id)) return false;

            try
            {
                var root = GetLayerRootFolder(layer.Id);
                if (Directory.Exists(root)) Directory.Delete(root, true);

                layer.IsDownloaded = false;
                layer.SizeGb = null;
                layer.DownloadedAt = null;
                return await UniTask.FromResult(true);
            }
            catch (Exception e)
            {
                ULog.Warning($"DeleteLayerAsync error: {e.Message}");
                return false;
            }
        }

        public string GetLocalUrlIfDownloaded(BaseLayerCombined layer)
        {
            var root = GetLayerRootFolder(layer.Id);
            if (!Directory.Exists(root)) return null;

            if (layer.Type == BaseLayerType.Imagery)
            {
                var manifestPath = Path.Combine(root, "manifest.json");
                string ext = "png";
                try
                {
                    if (File.Exists(manifestPath))
                    {
                        var mf = JsonConvert.DeserializeObject<LayerManifest>(File.ReadAllText(manifestPath));
                        ext = mf?.Imagery?.Ext ?? (InferImageExtensionFromTemplateOrUrl(layer.Href) ?? "png");
                    }
                    else
                    {
                        ext = InferImageExtensionFromTemplateOrUrl(layer.Href) ?? "png";
                    }
                }
                catch { /* keep default */ }

                return ToFileUrl(root) + $"/{{z}}/{{x}}/{{y}}.{ext}";
            }

            var tileset = Path.Combine(root, "tileset.json");
            if (!File.Exists(tileset)) return null;

            // Require at least one known content file next to tileset.json
            string[] exts = { ".b3dm", ".i3dm", ".cmpt", ".glb", ".gltf", ".subtree", ".terrain", ".vctr", ".pnts" };
            bool hasContent = false;
            try
            {
                foreach (var f in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
                {
                    var ext = Path.GetExtension(f).ToLowerInvariant();
                    if (Array.IndexOf(exts, ext) >= 0) { hasContent = true; break; }
                }
            }
            catch { /* ignore */ }

            return hasContent ? ToFileUrl(tileset) : null;
        }


        //  3D Tiles / Terrain (explicit references best-effort) 

        private async UniTask<bool> Download3DTilesAsync(BaseLayerCombined layer, CancellationToken ct)
        {
            if (layer == null || string.IsNullOrWhiteSpace(layer.Id) || string.IsNullOrWhiteSpace(layer.Href))
            {
                ULog.Warning("Invalid layer for download.");
                return false;
            }

            var root = GetLayerRootFolder(layer.Id);
            EnsureDirectory(root);

            var tilesetUrl = layer.Href;
            var tilesetLocal = Path.Combine(root, "tileset.json");

            // Download tileset.json (with resource headers – no bearer for SAS/external)
            var headers = BuildResourceHeaders(tilesetUrl);
            var tilesetResp = await _http.GetAsync(tilesetUrl, headers, ct);

            // Retry once without Authorization if 401/403 and we sent bearer
            if (!tilesetResp.IsSuccess && (tilesetResp.StatusCode == 401 || tilesetResp.StatusCode == 403) &&
                headers.ContainsKey("Authorization"))
            {
                headers.Remove("Authorization");
                tilesetResp = await _http.GetAsync(tilesetUrl, headers, ct);
            }

            if (!tilesetResp.IsSuccess || string.IsNullOrEmpty(tilesetResp.Data))
            {
                ULog.Warning(
                    $"Failed to download tileset.json for '{layer.Name}'. Status: {tilesetResp.StatusCode}. Error: {tilesetResp.ErrorMessage}");
                return false;
            }

            try
            {
                //AtomicWrite(tilesetLocal, System.Text.Encoding.UTF8.GetBytes(tilesetResp.Data));
                await AtomicWriteAsync(tilesetLocal, Encoding.UTF8.GetBytes(tilesetResp.Data), ct);
            }
            catch (Exception e)
            {
                ULog.Warning($"Failed writing tileset.json: {e.Message}");
                return false;
            }

            // Crawl & download referenced assets with bounded concurrency
            long totalBytes = new FileInfo(tilesetLocal).Length;

            // Base URL for resolving relatives (e.g., https://host/path/to/tileset.json -> https://host/path/to/)
            //var baseUrl = GetBaseUrl(tilesetUrl);
            var (basePath, baseQuery) = GetBaseUrlAndQuery(tilesetUrl);

            // Work queue & visited set to avoid duplicates
            //var queue = new Queue<(string baseUrl, string relative)>();
            var queue = new Queue<(string basePath, string baseQuery, string relative)>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Seed queue with references from the root tileset
            try
            {
                var doc = JObject.Parse(tilesetResp.Data);
                //EnqueueTilesetReferences(doc, queue, visited, baseUrl);
                EnqueueTilesetReferences(doc, queue, visited, basePath, baseQuery);
            }
            catch (Exception e)
            {
                ULog.Warning($"Failed to parse tileset.json for enqueue: {e.Message}");
            }

            // Worker counter
            int activeWorkers = 0;
            int errors = 0;
            const int errorLimit = 200; // be tolerant but not infinite

            // Local function to schedule one asset download
            async UniTask DownloadOneAsync((string basePath, string baseQuery, string relative) item)
            {
                try
                {
                    var url = CombineUrlPreservingQuery(item.basePath, item.baseQuery, item.relative);
                    var local = Path.Combine(root, NormalizeRelativePath(item.relative));
                    EnsureDirectory(Path.GetDirectoryName(local));

                    // Skip if we already have a healthy file
                    if (HasHealthyFile(local)) return;

                    // Respect concurrency & cancellation
                    await _downloadSemaphore.WaitAsync(ct);
                    try
                    {
                        var hdrs = BuildResourceHeaders(url);
                        var resp = await _http.GetBytesAsync(url, hdrs, ct);

                        // Retry without bearer if 401/403 and we had sent it
                        if (!resp.IsSuccess && (resp.StatusCode == 401 || resp.StatusCode == 403) &&
                            hdrs.ContainsKey("Authorization"))
                        {
                            var noAuth = new Dictionary<string, string>(hdrs);
                            noAuth.Remove("Authorization");
                            resp = await _http.GetBytesAsync(url, noAuth, ct);
                        }

                        if (!resp.IsSuccess || resp.Bytes == null || resp.Bytes.Length == 0)
                        {
                            Interlocked.Increment(ref errors);
                            return;
                        }

                        if (!HasSufficientDiskSpaceHardStop(local))
                        {
                            throw new IOException("Low disk space, aborting download.");
                        }

                        //AtomicWrite(local, resp.Bytes);
                        await AtomicWriteAsync(local, resp.Bytes, ct);
                        Interlocked.Add(ref totalBytes, resp.Bytes.LongLength);

                        // If this asset is JSON-like, parse & enqueue nested refs
                        if (IsJsonAsset(local))
                        {
                            try
                            {
                                var text = System.Text.Encoding.UTF8.GetString(resp.Bytes);
                                var j = JObject.Parse(text);
                                //EnqueueTilesetReferences(j, queue, visited, GetBaseUrl(url));
                                var (nestedBasePath, nestedBaseQuery) = GetBaseUrlAndQuery(url);
                                EnqueueTilesetReferences(j, queue, visited, nestedBasePath, nestedBaseQuery);
                            }
                            catch
                            {
                                // Best-effort: ignore JSON parse errors on content that isn't actual tileset/gltf JSON
                            }
                        }
                        // If glTF, we may need to parse buffers/images referenced inside – glTF JSON is handled above.
                        // Binary glb won't expose internal refs here (by design). That’s OK; external refs are covered by JSON paths.
                    }
                    finally
                    {
                        _downloadSemaphore.Release();
                    }
                }
                catch (OperationCanceledException)
                {
                    /* propagate via ct */
                    throw;
                }
                catch (Exception ex)
                {
                    ULog.Warning($"Asset download error '{item.relative}': {ex.Message}");
                    Interlocked.Increment(ref errors);
                }
                finally
                {
                    Interlocked.Decrement(ref activeWorkers);
                }
            }

            // Pump the queue until empty or cancelled
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                (string basePath, string baseQuery, string relative) next;
                lock (queue)
                {
                    if (queue.Count == 0) break;
                    next = queue.Dequeue();
                    Interlocked.Increment(ref activeWorkers);
                }

                // Schedule and yield control to avoid blocking the main thread
                _ = DownloadOneAsync(next);
                await UniTask.Yield(PlayerLoopTiming.Update, ct);

                // Backpressure: if there are too many active workers already scheduled, give them time
                while (Volatile.Read(ref activeWorkers) >
                       64) // arbitrary in-flight cap above semaphore to limit scheduling burst
                {
                    ct.ThrowIfCancellationRequested();
                    await UniTask.Delay(10, cancellationToken: ct);
                }

                if (errors > errorLimit)
                {
                    ULog.Warning($"Many asset errors ({errors}). Continuing but result may be incomplete.");
                    break;
                }
            }

            // Wait for all in-flight workers to drain
            while (Volatile.Read(ref activeWorkers) > 0)
            {
                ct.ThrowIfCancellationRequested();
                await UniTask.Delay(25, cancellationToken: ct);
            }

            // Write manifest (atomic)
            var manifestPath = Path.Combine(root, "manifest.json");
            var mf = new LayerManifest
            {
                Id = layer.Id,
                Name = layer.Name,
                Href = layer.Href,
                Type = layer.Type == BaseLayerType.Terrain ? "TERRAIN" : "TILES3D",
                DownloadedAtUtc = DateTime.UtcNow,
                TotalBytes = totalBytes,
                Imagery = null,
                Note = "3D Tiles/Terrain offline mirror (explicit references)",
                Access = layer.IsRestricted ? "RESTRICTED" : "PUBLIC"
            };

            try
            {
                //AtomicWrite(manifestPath, System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mf, Formatting.Indented)));
                await AtomicWriteAsync(manifestPath, System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mf, Formatting.Indented)), ct);
            }
            catch (Exception e)
            {
                ULog.Warning($"Failed writing 3D tiles manifest: {e.Message}");
            }

            return true;
        }


        //  Helpers 

        // True for .json and .gltf (text that may contain nested references)
        private static bool IsJsonAsset(string pathOrName)
        {
            if (string.IsNullOrWhiteSpace(pathOrName)) return false;
            var p = pathOrName.ToLowerInvariant();
            return p.EndsWith(".json") || p.EndsWith(".gltf");
        }

        private static (string BasePath, string BaseQuery) GetBaseUrlAndQuery(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var u))
            {
                // Parent folder (no filename), but keep original query
                var parent = new Uri(u, "."); // points to the containing folder
                var basePath = parent.GetLeftPart(UriPartial.Path);
                var baseQuery = u.Query; // includes leading "?" or empty
                return (basePath, baseQuery);
            }

            // Fallback for non-absolute URLs
            var i = url?.LastIndexOf('/') ?? -1;
            var path = (i >= 0) ? url.Substring(0, i + 1) : (url ?? string.Empty);
            // No reliable query parsing for non-absolute; assume none
            return (path, string.Empty);
        }

       
        private static string CombineUrlPreservingQuery(string basePath, string baseQuery, string relativeOrAbsolute)
        {
            if (string.IsNullOrWhiteSpace(relativeOrAbsolute)) return basePath + baseQuery;

            // Absolute child? Return as-is.
            if (Uri.TryCreate(relativeOrAbsolute, UriKind.Absolute, out var abs))
                return abs.AbsoluteUri;

            // Build absolute from basePath + relative
            if (!basePath.EndsWith("/")) basePath += "/";
            var combined = basePath + relativeOrAbsolute.TrimStart('/');

            // If the child already has a query, don't touch it.
            if (combined.Contains("?")) return combined;

            // If parent had a query (e.g., SAS), append it so child inherits access
            if (!string.IsNullOrEmpty(baseQuery))
                combined += baseQuery;

            return combined;
        }

        // Make a safe local filesystem path from a tileset-relative path
        private static string NormalizeRelativePath(string rel)
        {
            if (string.IsNullOrWhiteSpace(rel)) return string.Empty;
            rel = rel.Replace('\\', '/');
            var parts = rel.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var safe = parts.Where(p => p != "." && p != "..").ToArray();
            return string.Join(Path.DirectorySeparatorChar.ToString(), safe);
        }

        // Parse a tileset/glTF JSON and enqueue referenced assets
        private static void EnqueueTilesetReferences(
            JObject doc,
            Queue<(string basePath, string baseQuery, string relative)> queue,
            HashSet<string> visited,
            string basePath,
            string baseQuery)
        {
            if (doc == null) return;

            void TryEnqueue(string rel)
            {
                if (string.IsNullOrWhiteSpace(rel)) return;
                var key = $"{basePath}{baseQuery}|{rel}";
                if (visited.Add(key))
                {
                    lock (queue)
                    {
                        queue.Enqueue((basePath, baseQuery, rel));
                    }
                }
            }

            // Tileset paths (3D Tiles 1.0/1.1)
            void ScanTile(JToken tile, string localBase)
            {
                if (tile == null) return;

                // content.uri or content.url
                var content = tile["content"];
                if (content != null)
                {
                    var uri = content["uri"]?.Value<string>() ?? content["url"]?.Value<string>();
                    if (!string.IsNullOrWhiteSpace(uri)) TryEnqueue(uri);
                }

                // contents[] array (multiple contents)
                if (tile["contents"] is JArray contents)
                {
                    foreach (var c in contents)
                    {
                        var uri = c?["uri"]?.Value<string>() ?? c?["url"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(uri)) TryEnqueue(uri);
                    }
                }

                // Implicit tiling subtrees
                var subtreeUri = tile["extensions"]?["3DTILES_implicit_tiling"]?["subtrees"]?["uri"]?.Value<string>();
                if (!string.IsNullOrWhiteSpace(subtreeUri))
                {
                    TryEnqueue(subtreeUri);
                }

                // Children
                if (tile["children"] is JArray children)
                {
                    foreach (var ch in children) ScanTile(ch, localBase);
                }
            }

            // Entry points
            var root = doc["root"];
            if (root != null) ScanTile(root, basePath);

            // glTF JSON (buffers, images, external BINs)
            if (doc["asset"]?["version"] != null && (doc["buffers"] != null || doc["images"] != null))
            {
                // buffers
                if (doc["buffers"] is JArray buffers)
                {
                    foreach (var b in buffers)
                    {
                        var uri = b?["uri"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(uri) &&
                            !uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                            TryEnqueue(uri);
                    }
                }

                // images
                if (doc["images"] is JArray images)
                {
                    foreach (var im in images)
                    {
                        var uri = im?["uri"]?.Value<string>();
                        if (!string.IsNullOrWhiteSpace(uri) &&
                            !uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                            TryEnqueue(uri);
                    }
                }
                // external .bin buffers are handled by buffers[].uri above
            }
        }


        private Dictionary<string, string> BuildAuthHeaders()
        {
            var headers = new Dictionary<string, string> { ["Accept"] = "*/*" };
            var accessToken = _auth?.GetTokenStorageProvider()?.GetAccessToken();
            if (!string.IsNullOrWhiteSpace(accessToken))
                headers["Authorization"] = $"Bearer {accessToken}";
            return headers;
        }

        private string GetLayerRootFolder(string layerId)
        {
            return Path.Combine(DownloadRoot, layerId);
        }

        private static void EnsureDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            try { if (!Directory.Exists(path)) Directory.CreateDirectory(path); }
            catch (Exception e)
            {
                ULog.Warning($"EnsureDirectory failed for '{path}': {e.Message}");
                throw;
            }
        }

        private static long SumFolderBytes(string root)
        {
            if (!Directory.Exists(root)) return 0;
            long total = 0;
            foreach (var f in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                try { total += new FileInfo(f).Length; } catch { }
            }
            return total;
        }

        private static string ToFileUrl(string fullPath)
        {
            var uri = new Uri(fullPath, UriKind.Absolute);
            return uri.AbsoluteUri;
        }

        private static string BytesToHuman(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            double kb = bytes / 1024.0;
            if (kb < 1024) return $"{kb:F1} KB";
            double mb = kb / 1024.0;
            if (mb < 1024) return $"{mb:F1} MB";
            double gb = mb / 1024.0;
            return $"{gb:F2} GB";
        }

        private static bool HasSufficientDiskSpace(string pathWithinTarget, long neededBytes)
        {
            if (neededBytes <= 0) return true;
            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(pathWithinTarget)));
                long free = drive.AvailableFreeSpace;
                return free > (long)(neededBytes * 1.05);
            }
            catch { return true; }
        }

        private static bool HasSufficientDiskSpaceHardStop(string pathWithinTarget, long reserveBytes = 512L * 1024 * 1024)
        {
            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(pathWithinTarget)));
                return drive.AvailableFreeSpace > reserveBytes;
            }
            catch { return true; }
        }

        private static IEnumerable<(int Z, int X, int Y)> EnumerateTiles(double minLon, double minLat, double maxLon, double maxLat, int minZ, int maxZ)
        {
            minLon = Mathf.Clamp((float)minLon, -180f, 180f);
            maxLon = Mathf.Clamp((float)maxLon, -180f, 180f);
            minLat = Mathf.Clamp((float)minLat, -85.05112878f, 85.05112878f);
            maxLat = Mathf.Clamp((float)maxLat, -85.05112878f, 85.05112878f);

            for (int z = minZ; z <= maxZ; z++)
            {
                (int x0, int y0) = LonLatToTileXY(minLon, maxLat, z);
                (int x1, int y1) = LonLatToTileXY(maxLon, minLat, z);
                if (x1 < x0) { var t = x0; x0 = x1; x1 = t; }
                if (y1 < y0) { var t = y0; y0 = y1; y1 = t; }

                for (int x = x0; x <= x1; x++)
                    for (int y = y0; y <= y1; y++)
                        yield return (z, x, y);
            }
        }

        private static (int X, int Y) LonLatToTileXY(double lon, double lat, int z)
        {
            var x = (int)Math.Floor((lon + 180.0) / 360.0 * (1 << z));
            var latRad = lat * Math.PI / 180.0;
            var y = (int)Math.Floor((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * (1 << z));
            return (x, y);
        }

        private static string ResolveTemplate(string template, int z, int x, int y)
        {
            var reverseY = ((1 << z) - 1) - y;
            var url = template
                .Replace("{z}", z.ToString(CultureInfo.InvariantCulture))
                .Replace("{x}", x.ToString(CultureInfo.InvariantCulture))
                .Replace("{y}", y.ToString(CultureInfo.InvariantCulture))
                .Replace("{reverseY}", reverseY.ToString(CultureInfo.InvariantCulture));
            return url;
        }

        private static string InferImageExtensionFromTemplateOrUrl(string url)
        {
            var qIdx = url.IndexOf('?', StringComparison.Ordinal);
            if (qIdx >= 0) url = url.Substring(0, qIdx);
            var dot = url.LastIndexOf('.');
            if (dot >= 0 && dot < url.Length - 1)
            {
                var ext = url.Substring(dot + 1).ToLowerInvariant();
                if (ext.Length <= 4) return ext;
            }
            return null;
        }

        private static async UniTask AtomicWriteAsync(string path, byte[] data, CancellationToken ct)
        {
            var dir = Path.GetDirectoryName(path);
            EnsureDirectory(dir);

            var tmp = path + ".tmp";

            // async file write
            using (var fs = new FileStream(
                       tmp,
                       FileMode.Create,
                       FileAccess.Write,
                       FileShare.None,
                       bufferSize: 64 * 1024,
                       options: FileOptions.Asynchronous))
            {
                await fs.WriteAsync(data, 0, data.Length, ct);
                await fs.FlushAsync(ct);
            }

            // atomic swap (falls back if Replace not available)
            try
            {
                File.Replace(tmp, path, null); // atomic on Windows; on other platforms it may throw
            }
            catch
            {
                try { if (File.Exists(path)) File.Delete(path); } catch { /* best effort */ }
                File.Move(tmp, path);
            }
        }

        private static void AtomicWrite(string path, byte[] data)
        {
            var dir = Path.GetDirectoryName(path);
            EnsureDirectory(dir);
            var tmp = path + ".tmp";
            File.WriteAllBytes(tmp, data);
            if (File.Exists(path))
            {
                try { File.Replace(tmp, path, null); }
                catch
                {
                    try { File.Delete(path); } catch { }
                    File.Move(tmp, path);
                }
            }
            else
            {
                File.Move(tmp, path);
            }
        }

        private static bool HasHealthyFile(string path, long minBytes = 1)
        {
            try
            {
                var fi = new FileInfo(path);
                return fi.Exists && fi.Length >= minBytes;
            }
            catch { return false; }
        }

        private async UniTask<long> EstimateTilesContentLengthAsync(
            string template,
            IEnumerable<(int Z, int X, int Y)> tiles,
            Dictionary<string, string> headers,
            CancellationToken ct
        )
        {
            var headHeaders = BuildResourceHeaders(template);
            const int maxSamples = 200;
            long sampledBytes = 0;
            int samples = 0;

            // If HttpClient supports HEAD, we’ll use it; otherwise return 0 (skip estimate)
            var httpWithHead = _http as HttpClientWithRetry;

            foreach (var t in tiles.Take(maxSamples))
            {
                ct.ThrowIfCancellationRequested();
                if (httpWithHead == null) break;

                var url = ResolveTemplate(template, t.Z, t.X, t.Y);
                var headResp = await httpWithHead.HeadAsync(url, headHeaders, ct);
                if ((headResp == null || !headResp.IsSuccess) && headHeaders.ContainsKey("Authorization"))
                {
                    var noAuth = new Dictionary<string, string>(headHeaders);
                    noAuth.Remove("Authorization");
                    headResp = await httpWithHead.HeadAsync(url, noAuth, ct);
                }
                if (headResp != null && headResp.IsSuccess && headResp.ContentLength.HasValue)
                {
                    sampledBytes += Math.Max(0, headResp.ContentLength.Value);
                    samples++;
                }
            }

            if (samples == 0) return 0;

            var avg = sampledBytes / samples;
            var count = tiles.LongCount();
            return avg * count;
        }

        private Dictionary<string, string> BuildApiHeaders()
        {
            return new Dictionary<string, string> { ["Accept"] = "application/json" };
        }

        private Dictionary<string, string> BuildResourceHeaders(string url)
        {
            var headers = new Dictionary<string, string> { ["Accept"] = "*/*" };

            // If this URL is on our API host and NOT a SAS url, we may include bearer.
            // Otherwise (SAS/public/external CDNs), skip Authorization.
            if (ShouldSendBearerFor(url))
            {
                var token = _auth?.GetTokenStorageProvider()?.GetAccessToken();
                if (!string.IsNullOrWhiteSpace(token))
                    headers["Authorization"] = $"Bearer {token}";
            }

            return headers;
        }

        private bool ShouldSendBearerFor(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            // if SAS-style signature present -> do not send bearer
            if (url.Contains("sig=", StringComparison.OrdinalIgnoreCase)) return false;

            // Parse host and compare to our API base host
            try
            {
                var apiBase = _appConfig?.ServerConfig?.ApiBaseUrl;
                if (string.IsNullOrWhiteSpace(apiBase)) return false;

                var apiHost = new Uri(apiBase).Host;
                var u = new Uri(url, UriKind.Absolute);
                var sameHost = string.Equals(u.Host, apiHost, StringComparison.OrdinalIgnoreCase);

                // Only send bearer for same-host resources (and non-SAS)
                return sameHost;
            }
            catch { return false; }
        }
    }
}