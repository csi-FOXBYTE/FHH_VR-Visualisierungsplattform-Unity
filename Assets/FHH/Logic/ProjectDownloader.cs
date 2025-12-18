using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using FHH.Logic.Models;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace FHH.Logic
{
    public sealed class ProjectDownloader
    {
        private readonly string _rootPath;
        private readonly IAuthenticationService _auth;
        private readonly IHttpClient _http;

        public ProjectDownloader(IAuthenticationService auth, IHttpClient http)
        {
            _auth = auth;
            _http = http;
            _rootPath = Path.Combine(Application.persistentDataPath, "Projects");
        }

        private string GetProjectFolder(string projectId)
        {
            // optionally sanitize projectId
            return Path.Combine(_rootPath, projectId);
        }

        private string GetOfflineJsonPath(string projectId)
        {
            return Path.Combine(GetProjectFolder(projectId), "project.offline.json");
        }

        public bool IsProjectCached(string projectId)
        {
            return File.Exists(GetOfflineJsonPath(projectId));
        }

        public async UniTask<Project> LoadProjectAsync(string projectId, CancellationToken token)
        {
            var path = GetOfflineJsonPath(projectId);
            if (!File.Exists(path))
            {
                ULog.Warning($"Offline project not found for {projectId} at {path}");
                return null;
            }

            var json = await ReadAllTextAsync(path, token);
            return Project.FromJson(json);
        }

        private string GetDownloadUrl(Project project, string modelUrl)
        {
            if (string.IsNullOrWhiteSpace(modelUrl))
            {
                return null;
            }

            // If URL already has a query (e.g., SAS already attached), use it as-is
            if (modelUrl.Contains("?"))
            {
                return modelUrl;
            }

            if (string.IsNullOrWhiteSpace(project.ProjectSasQueryParameters))
            {
                // No SAS known, fall back to raw URL
                return modelUrl;
            }

            var sas = project.ProjectSasQueryParameters.TrimStart('?');
            var separator = modelUrl.Contains("?") ? "&" : "?";
            return modelUrl + separator + sas;
        }

        public async UniTask<bool> SaveProjectAsync(Project project, string originalJson, CancellationToken token)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));
            if (string.IsNullOrWhiteSpace(originalJson)) throw new ArgumentException("originalJson is null or empty", nameof(originalJson));

            var folder = GetProjectFolder(project.Id);
            Directory.CreateDirectory(folder);

            // Save original JSON for debugging if needed
            var remoteJsonPath = Path.Combine(folder, "project.remote.json");
            await WriteAllTextAsync(remoteJsonPath, originalJson, token);

            var modelFolder = Path.Combine(folder, "models");
            Directory.CreateDirectory(modelFolder);

            var uniqueUrls = project.Variants?
                .Where(v => v.Models != null)
                .SelectMany(v => v.Models)
                .Select(m => m.Url)
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Distinct()
                .ToList() ?? new List<string>();

            var urlToFileUri = new Dictionary<string, string>();

            foreach (var originalUrl in uniqueUrls)
            {
                token.ThrowIfCancellationRequested();

                var downloadUrl = GetDownloadUrl(project, originalUrl);
                if (string.IsNullOrWhiteSpace(downloadUrl))
                {
                    ULog.Warning($"Model URL is empty for project {project.Id}");
                    continue;
                }

                var localFilePath = await DownloadModelAsync(downloadUrl, modelFolder, token);
                if (string.IsNullOrEmpty(localFilePath))
                {
                    ULog.Warning($"Failed to download model for URL {downloadUrl}");
                    continue;
                }

                var fileUri = new Uri(localFilePath).AbsoluteUri;
                urlToFileUri[originalUrl] = fileUri;
            }

            // Rewrite JSON to file:// URIs for models
            var dto = JsonConvert.DeserializeObject<ProjectDto>(originalJson);
            if (dto == null) throw new JsonException("Failed to deserialize ProjectDto for offline storage.");

            if (dto.Variants != null)
            {
                foreach (var v in dto.Variants)
                {
                    if (v.Models == null) continue;

                    foreach (var m in v.Models)
                    {
                        if (string.IsNullOrWhiteSpace(m.Url)) continue;

                        if (urlToFileUri.TryGetValue(m.Url, out var localUri))
                        {
                            m.Url = localUri;
                        }
                    }
                }
            }

            var offlineJson = JsonConvert.SerializeObject(dto);
            var offlinePath = GetOfflineJsonPath(project.Id);
            await WriteAllTextAsync(offlinePath, offlineJson, token);

            return true;
        }

        public async UniTask DeleteProjectAsync(string projectId, CancellationToken token)
        {
            var folder = GetProjectFolder(projectId);
            if (!Directory.Exists(folder))
            {
                return;
            }

            await UniTask.RunOnThreadPool(() =>
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception ex)
                {
                    ULog.Error($"Failed to delete offline project folder {folder}: {ex.Message}");
                }
            }, cancellationToken: token);
        }

        public async UniTask<List<Project>> GetOfflineProjectsAsync(CancellationToken token)
        {
            var result = new List<Project>();

            if (!Directory.Exists(_rootPath))
            {
                return result;
            }

            var projectFolders = Directory.GetDirectories(_rootPath);
            foreach (var folder in projectFolders)
            {
                token.ThrowIfCancellationRequested();

                var offlinePath = Path.Combine(folder, "project.offline.json");
                if (!File.Exists(offlinePath))
                {
                    continue;
                }

                try
                {
                    var json = await ReadAllTextAsync(offlinePath, token);
                    var project = Project.FromJson(json);
                    if (project != null)
                    {
                        result.Add(project);
                    }
                }
                catch (Exception ex)
                {
                    ULog.Error($"Failed to read offline project from {offlinePath}: {ex.Message}");
                }
            }

            return result;
        }

        private async UniTask<string> DownloadModelAsync(string url, string modelFolder, CancellationToken token)
        {
            try
            {
                // We intentionally do NOT send an Authorization header here
                // These URLs already work for glTFast as-is (typically SAS / signed URLs)
                var result = await _http.GetBytesAsync(url, null, token);
                if (!result.IsSuccess || result.Bytes == null || result.Bytes.Length == 0)
                {
                    ULog.Error($"Failed to download model from {url}: {result.ErrorMessage}");
                    return null;
                }

                var data = result.Bytes;

                var uri = new Uri(url);
                var fileName = Path.GetFileName(uri.LocalPath);
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = $"model_{Guid.NewGuid():N}.glb";
                }

                var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
                if (extension == ".gltf")
                {
                    // Handle glTF + external resources
                    var json = Encoding.UTF8.GetString(data);
                    var modelBaseFolder = Path.Combine(modelFolder, Path.GetFileNameWithoutExtension(fileName));
                    Directory.CreateDirectory(modelBaseFolder);

                    var localGltfPath = Path.Combine(modelBaseFolder, fileName);
                    await WriteAllTextAsync(localGltfPath, json, token);

                    await DownloadGltfExternalResourcesAsync(json, url, modelBaseFolder, token);

                    return localGltfPath;
                }
                else
                {
                    // Default: treat as .glb (self-contained) or other binary
                    var localPath = Path.Combine(modelFolder, fileName);
                    await WriteAllBytesAsync(localPath, data, token);
                    return localPath;
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"Exception while downloading model from {url}: {ex.Message}");
                return null;
            }
        }

        private async UniTask DownloadGltfExternalResourcesAsync(string gltfJson, string rootUrl, string targetFolder,
            CancellationToken token)
        {
            try
            {
                var jObj = JObject.Parse(gltfJson);

                var bufferUris = jObj["buffers"]?
                    .Select(b => (string)b["uri"])
                    .Where(u => !string.IsNullOrWhiteSpace(u)) ?? Enumerable.Empty<string>();

                var imageUris = jObj["images"]?
                    .Select(i => (string)i["uri"])
                    .Where(u => !string.IsNullOrWhiteSpace(u)) ?? Enumerable.Empty<string>();

                var uris = bufferUris.Concat(imageUris).Distinct();

                var rootUri = new Uri(rootUrl);
                var rootSasQuery = rootUri.Query; // includes leading '?', may be empty

                foreach (var relativeUri in uris)
                {
                    token.ThrowIfCancellationRequested();

                    if (relativeUri.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    {
                        // embedded data, nothing to download
                        continue;
                    }

                    var combinedUri = new Uri(rootUri, relativeUri);

                    // If combined URI has no query but the root had SAS, append the SAS
                    if (!string.IsNullOrEmpty(rootSasQuery) && string.IsNullOrEmpty(combinedUri.Query))
                    {
                        var builder = new UriBuilder(combinedUri)
                        {
                            Query = rootSasQuery.TrimStart('?')
                        };
                        combinedUri = builder.Uri;
                    }

                    var combinedUrl = combinedUri.ToString();

                    var localPath = Path.Combine(targetFolder, relativeUri.Replace('/', Path.DirectorySeparatorChar));

                    var directory = Path.GetDirectoryName(localPath);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    await DownloadToFileAsync(combinedUrl, localPath, token);
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"Failed to download external resources for glTF: {ex.Message}");
            }
        }

        private async UniTask DownloadToFileAsync(string url, string localPath, CancellationToken token)
        {
            try
            {
                var result = await _http.GetBytesAsync(url, null, token);
                if (!result.IsSuccess || result.Bytes == null || result.Bytes.Length == 0)
                {
                    ULog.Error($"Failed to download glTF resource from {url}: {result.ErrorMessage}");
                    return;
                }

                var directory = Path.GetDirectoryName(localPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await WriteAllBytesAsync(localPath, result.Bytes, token);
            }
            catch (Exception ex)
            {
                ULog.Error($"Exception while downloading glTF resource from {url}: {ex.Message}");
            }
        }

        private async UniTask<string> ReadAllTextAsync(string path, CancellationToken token)
        {
            return await UniTask.RunOnThreadPool(() => File.ReadAllText(path), cancellationToken: token);
        }

        private async UniTask WriteAllTextAsync(string path, string text, CancellationToken token)
        {
            await UniTask.RunOnThreadPool(() => File.WriteAllText(path, text), cancellationToken: token);
        }

        private async UniTask WriteAllBytesAsync(string path, byte[] data, CancellationToken token)
        {
            await UniTask.RunOnThreadPool(() => File.WriteAllBytes(path, data), cancellationToken: token);
        }
    }
}