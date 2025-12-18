using CesiumForUnity;
using Cysharp.Threading.Tasks;
using FHH.Logic.Cesium;
using FHH.Logic.Models;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foxbyte.Core.Services.Permission;
using GLTFast;
using UnityEngine;
using UnityEngine.Splines;

namespace FHH.Logic
{
    [Serializable]
    public sealed class ServerBaseLayerDto
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("name")] public string Name;
        [JsonProperty("href")] public string Href;
        [JsonProperty("type")] public string Type; // "TILES3D" | "IMAGERY" | "TERRAIN"
        [JsonProperty("description")] public string Description;
    }

    public sealed class BaseLayerCombined
    {
        public string Id;
        public string Name;
        public string Href;
        public BaseLayerType Type;
        public string Description;

        public bool IsVisible;
        public bool IsDownloaded;
        public float? SizeGb;
        public DateTime? DownloadedAt;

        public bool IsRestricted;

        public GameObject Instance; // The GO this layer is tied to
        public Component BoundComponent;   // Tileset or overlay component bound
        public Component OverlayForPolygons; // Dedicated overlay to host polygons for this layer (if applicable)
    }

    public interface ILayerDownloader
    {
        UniTask RefreshDownloadedStatusAsync(IEnumerable<BaseLayerCombined> layers);
        UniTask<bool> DownloadLayerAsync(BaseLayerCombined layer, CancellationToken ct = default);
        UniTask<bool> DownloadImageryAsync(BaseLayerCombined layer, double minLon, double minLat, double maxLon, double maxLat, int minZoom, int maxZoom, CancellationToken ct = default);
        UniTask<bool> DeleteLayerAsync(BaseLayerCombined layer);
        string GetLocalUrlIfDownloaded(BaseLayerCombined layer);
        string DownloadRoot { get; }
    }

    public class LayerManager : MonoBehaviour
    {
        public static LayerManager Instance { get; private set; }

        public event Action<IReadOnlyList<BaseLayerCombined>> OnCombinedLayersChanged;
        public event Action<Project> OnProjectChanged;
        public event Action<ProjectVariant> OnVariantChanged;
        public event Action OnProjectUnloaded;
        public event Action OnProjectUnloadingStarted;

        [Header("Templates in Scene")]
        [SerializeField] private GameObject _cesiumCityTemplateInScene;
        [SerializeField] private GameObject _cesiumTerrainTemplateInScene;
        [SerializeField] private GameObject _cesiumTreesTemplateInScene;

        [Header("External Providers")]
        [SerializeField] private CesiumTilesLoadingProgressProvider _progressProvider;

        [Header("Access Control")]
        [SerializeField] private bool _restrictedVisibleByDefault = false;

        //private readonly List<BaseLayerCombined> _publicBaseLayers = new List<BaseLayerCombined>();
        //private readonly List<BaseLayerCombined> _restrictedBaseLayers = new List<BaseLayerCombined>();

        //public bool HasRestrictedLayersLoaded => _restrictedBaseLayers.Count > 0;
        //public IReadOnlyList<BaseLayerCombined> GetRestrictedBaseLayers() => _restrictedBaseLayers;

        private IAuthenticationService _auth;
        private IHttpClient _http;
        private AppConfig _appConfig;

        private Transform _geoRefParent;

        private readonly List<BaseLayerCombined> _combinedLayers = new List<BaseLayerCombined>();
        private readonly Dictionary<string, BaseLayerCombined> _byId = new Dictionary<string, BaseLayerCombined>(StringComparer.OrdinalIgnoreCase);

        public Project CurrentProject;
        public ProjectVariant CurrentVariant;

        private readonly List<Component> _activeVariantPolygons = new List<Component>();
        private readonly List<GameObject> _activeVariantModels = new List<GameObject>();

        private ILayerDownloader _downloader;

        private readonly SemaphoreSlim _opLock = new SemaphoreSlim(1, 1);
        private int _opVersion = 0;

        private bool _instancesInitialized = false;
        private bool _restrictedLoadedOnce = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitAsync().Forget();
        }

        private async UniTask InitAsync()
        {
            try
            {
                _auth = ServiceLocator.GetService<OAuth2AuthenticationService>();
                _http = new HttpClientWithRetry();
                _appConfig = ServiceLocator.GetService<ConfigurationService>().AppSettings;
                var perm = ServiceLocator.GetService<PermissionService>();
                if (perm != null)
                {
                    perm.UserChanged += OnUserChangedPermissionService;
                }
            }
            catch (Exception e)
            {
                ULog.Error($"LayerManager service init failed: {e.Message}");
            }

            var go = GameObject.FindGameObjectWithTag("CesiumGeoRef");
            if (go == null) ULog.Error("CesiumGeoRef parent not found. Scene setup incomplete.");
            _geoRefParent = go != null ? go.transform : null;

            _downloader = new LayerDownloader(_auth, _http, _appConfig);

            await InitializeGeneralLayersAsync();
            // Restricted: first from disk (always), then server if authenticated (metadata-only; no instantiation)
            await LoadRestrictedLayersAsync();
        }

        public IReadOnlyList<BaseLayerCombined> GetCombinedBaseLayers() => _combinedLayers;

       
        //  Public API

        public Project GetCurrentProject()
        {
            return CurrentProject;
        }

        public ProjectVariant GetCurrentVariant()
        {
            return CurrentVariant;
        }
        
        public async UniTask<bool> ToggleBaseLayerVisibilityAsync(string layerId, bool visible)
        {
            if (!_byId.TryGetValue(layerId, out var layer))
            {
                ULog.Warning($"Toggle requested for unknown layer id '{layerId}'.");
                return false;
            }

            if (layer.Type == BaseLayerType.Imagery)
            {
                var terrain = _combinedLayers.FirstOrDefault(x => x.Type == FHH.Logic.Models.BaseLayerType.Terrain && x.Instance != null);
                var overlay = terrain != null ? GetExistingUrlTemplateOverlay(terrain.Instance) : null;
                if (overlay == null) return false;

                if (visible)
                {
                    // Turn off any other imagery
                    foreach (var other in _combinedLayers.Where(x => x.Type == FHH.Logic.Models.BaseLayerType.Imagery && !ReferenceEquals(x, layer)))
                        other.IsVisible = false;

                    layer.IsVisible = true;

                    // Apply this imagery's URL to the single overlay and enable it
                    var local = _downloader.GetLocalUrlIfDownloaded(layer);
                    var url = string.IsNullOrEmpty(local) ? layer.Href : local;
                    SetOverlayTemplate(overlay, url);
                    if (overlay is Behaviour ob) ob.enabled = true;
                }
                else
                {
                    layer.IsVisible = false;

                    // If no imagery remains visible, blank the template and disable overlay
                    bool anyOtherOn = _combinedLayers.Any(x => x.Type == FHH.Logic.Models.BaseLayerType.Imagery && x.IsVisible);
                    if (!anyOtherOn)
                    {
                        SetOverlayTemplate(overlay, string.Empty); // <-- blank template on invisibility
                        if (overlay is Behaviour ob) ob.enabled = false;
                    }
                    else
                    {
                        // Another imagery is visible -> point overlay to that one explicitly
                        var active = _combinedLayers.First(x => x.Type == FHH.Logic.Models.BaseLayerType.Imagery && x.IsVisible);
                        var local = _downloader.GetLocalUrlIfDownloaded(active);
                        var url = string.IsNullOrEmpty(local) ? active.Href : local;
                        SetOverlayTemplate(overlay, url);
                        if (overlay is Behaviour ob) ob.enabled = true;
                    }
                }

                RaiseCombinedChanged();
                return true;
            }

            // Non-imagery → normal toggle
            if (layer.IsVisible == visible) return true;
            layer.IsVisible = visible;
            ApplyLayerVisibility(layer);
            RaiseCombinedChanged();
            return await UniTask.FromResult(true);
        }

       
        /// <summary>
        /// Download with CancellationToken support so downloads can be cancelled (e.g. when UI is closed).
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async UniTask<bool> DownloadLayerAsync(string layerId, CancellationToken ct)
        {
            if (!_byId.TryGetValue(layerId, out var layer))
            {
                ULog.Warning($"Download requested for unknown layer id '{layerId}'.");
                return false;
            }

            try
            {
                var ok = await _downloader.DownloadLayerAsync(layer, ct); 
                if (ok) await _downloader.RefreshDownloadedStatusAsync(_combinedLayers);
                ApplyLayerUrl(layer);
                RaiseCombinedChanged();
                return ok;
            }
            catch (OperationCanceledException)
            {
                ULog.Info($"Download canceled for '{layer.Name}' ({layer.Id}).");
                return false;
            }
            catch (Exception e)
            {
                ULog.Warning($"DownloadLayerAsync error: {e.Message}");
                return false;
            }
        }


        public async UniTask<bool> DeleteLayerAsync(string layerId)
        {
            if (!_byId.TryGetValue(layerId, out var layer))
            {
                ULog.Warning($"Delete requested for unknown layer id '{layerId}'.");
                return false;
            }

            try
            {
                var ok = await _downloader.DeleteLayerAsync(layer);
                if (ok)
                {
                    await _downloader.RefreshDownloadedStatusAsync(_combinedLayers);
                    ApplyLayerUrl(layer);
                    RaiseCombinedChanged();
                }
                return ok;
            }
            catch (Exception e)
            {
                ULog.Warning($"DeleteLayerAsync error: {e.Message}");
                return false;
            }
        }

        public async UniTask<bool> SetProjectAsync(Project project)
        {
            // check if same project
            if (ReferenceEquals(CurrentProject, project) ||
                (project != null && string.Equals(CurrentProject?.Id, project.Id, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            int version;
            await _opLock.WaitAsync();
            try { version = ++_opVersion; }
            finally { _opLock.Release(); }

            var prevProject = CurrentProject;
            var prevVariant = CurrentVariant;

            CurrentProject = project;
            CurrentVariant = project?.Variants?.FirstOrDefault();

            // Visibility policy: only layers included by the project are visible.
            HashSet<string> included = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (CurrentVariant?.BaseLayers != null)
            {
                foreach (var vb in CurrentVariant.BaseLayers)
                {
                    if (!string.IsNullOrWhiteSpace(vb?.Id)) included.Add(vb.Id);
                }
            }

            foreach (var l in _combinedLayers)
                l.IsVisible = included.Contains(l.Id);

            await RebuildSceneForCombinedAsync(CurrentVariant, version);

            RaiseCombinedChanged();

            if (!ReferenceEquals(prevProject, CurrentProject))
                OnProjectChanged?.Invoke(CurrentProject);

            if (!ReferenceEquals(prevVariant, CurrentVariant))
                OnVariantChanged?.Invoke(CurrentVariant);

            return true;
        }

        public async UniTask<bool> UnloadProjectAsync()
        {
            // check if no project
            if (CurrentProject == null)
            {
                return true;
            }
            
            //OnProjectUnloadingStarted?.Invoke();
            try { OnProjectUnloadingStarted?.Invoke(); }
            catch (Exception ex) { Debug.LogWarning($"[LayerManager] OnProjectUnloadingStarted handler threw:\n{ex}"); }

            if (_opLock == null)
            {
                Debug.LogWarning("[LayerManager] _opLock is null during UnloadProjectAsync");
                return true;
            }

            int version;
            await _opLock.WaitAsync();
            try { version = ++_opVersion; }
            finally { _opLock.Release(); }

            var prevProject = CurrentProject;
            var prevVariant = CurrentVariant;

            CurrentProject = null;
            CurrentVariant = null;

            if (_combinedLayers == null)
            {
                Debug.LogWarning("[LayerManager] _combinedLayers is null during UnloadProjectAsync");
            }
            else
            {

                foreach (var l in _combinedLayers)
                {
                    l.IsVisible = l.IsRestricted ? _restrictedVisibleByDefault : true;
                }

                foreach (var l in _combinedLayers)
                {
                    ApplyLayerUrl(l);
                    ApplyLayerVisibility(l);
                }
            }

            //SyncImageryOverlayToCurrentState();
            try { SyncImageryOverlayToCurrentState(); }
            catch (Exception ex) { Debug.LogWarning($"[LayerManager] SyncImageryOverlayToCurrentState threw:\n{ex}"); }

            //await RebuildSceneForCombinedAsync(null, version);
            ClearVariantPolygons();
            ClearVariantModels();

            RaiseCombinedChanged();

            if (!ReferenceEquals(prevProject, CurrentProject))
            {
                //OnProjectChanged?.Invoke(CurrentProject);
                try { OnProjectChanged?.Invoke(CurrentProject); }
                catch (Exception ex) { Debug.LogWarning($"[LayerManager] OnProjectChanged handler threw:\n{ex}"); }
            }

            if (!ReferenceEquals(prevVariant, CurrentVariant))
            {
                //OnVariantChanged?.Invoke(CurrentVariant);
                try { OnVariantChanged?.Invoke(CurrentVariant); }
                catch (Exception ex) { Debug.LogWarning($"[LayerManager] OnVariantChanged handler threw:\n{ex}"); }
            }

            //OnProjectUnloaded?.Invoke();
            try { OnProjectUnloaded?.Invoke(); }
            catch (Exception ex) { Debug.LogWarning($"[LayerManager] OnProjectUnloaded handler threw:\n{ex}"); }

            return true;
        }

        /// <summary>
        /// Set the active variant within the currently selected project.
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        public async UniTask<bool> SetVariantAsync(ProjectVariant variant)
        {
            // Must have a project already selected
            if (CurrentProject == null)
            {
                ULog.Warning("SetVariantAsync called but no project is currently set.");
                return false;
            }

            if (variant == null)
            {
                ULog.Warning("SetVariantAsync called with null variant.");
                return false;
            }

            // Ensure the provided variant belongs to the current project
            var belongsToProject = CurrentProject.Variants != null &&
                                   CurrentProject.Variants.Any(v => string.Equals(v?.Id, variant.Id, StringComparison.OrdinalIgnoreCase));

            if (!belongsToProject)
            {
                ULog.Warning($"Variant '{variant.Id}' does not belong to the current project '{CurrentProject.Id}'.");
                return false;
            }

            // No-op if it's already the active one
            if (ReferenceEquals(CurrentVariant, variant) ||
                (string.Equals(CurrentVariant?.Id, variant.Id, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            int version;
            await _opLock.WaitAsync();
            try { version = ++_opVersion; }
            finally { _opLock.Release(); }

            var prevVariant = CurrentVariant;
            CurrentVariant = variant;

            // Apply visibility policy for the new variant: only included base layers visible
            var included = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (CurrentVariant?.BaseLayers != null)
            {
                foreach (var vb in CurrentVariant.BaseLayers)
                {
                    if (!string.IsNullOrWhiteSpace(vb?.Id)) included.Add(vb.Id);
                }
            }

            foreach (var l in _combinedLayers)
                l.IsVisible = included.Contains(l.Id);

            await RebuildSceneForCombinedAsync(CurrentVariant, version);
            RaiseCombinedChanged();

            if (!ReferenceEquals(prevVariant, CurrentVariant))
                OnVariantChanged?.Invoke(CurrentVariant);

            return true;
        }

        /// <summary>
        /// Set the active variant by its identifier within the currently selected project.
        /// </summary>
        /// <param name="variantId"></param>
        /// <returns></returns>
        public async UniTask<bool> SetVariantByIdAsync(string variantId)
        {
            if (CurrentProject == null)
            {
                ULog.Warning("SetVariantByIdAsync called but no project is currently set.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(variantId) || CurrentProject.Variants == null)
                return false;

            var match = CurrentProject.Variants.FirstOrDefault(v =>
                string.Equals(v?.Id, variantId, StringComparison.OrdinalIgnoreCase));

            if (match == null)
            {
                ULog.Warning($"Variant '{variantId}' not found in current project '{CurrentProject.Id}'.");
                return false;
            }
            return await SetVariantAsync(match);
        }

        public async UniTask RefreshDownloadedStatusAsync() =>
            await _downloader.RefreshDownloadedStatusAsync(_combinedLayers);

        //  Initialization 

        private async UniTask InitializeGeneralLayersAsync()
        {
            List<ServerBaseLayerDto> serverPublic = null;

            try
            {
                var endpoint = _appConfig.ServerConfig.EndpointPath("BaseLayers");
                serverPublic = await GetDataFromPublicEndpointAsync<List<ServerBaseLayerDto>>(endpoint);
            }
            catch (Exception e)
            {
                ULog.Warning($"Fetching general base layers failed: {e.Message}");
            }

            _combinedLayers.Clear();
            _byId.Clear();

            if (serverPublic != null && serverPublic.Count > 0)
            {
                foreach (var s in serverPublic)
                {
                    if (s == null || string.IsNullOrWhiteSpace(s.Id) || string.IsNullOrWhiteSpace(s.Type)) continue;
                    if (!TryMapType(s.Type, out var t)) continue;

                    // terrain: ensure /layer.json
                    if (t == BaseLayerType.Terrain && !string.IsNullOrWhiteSpace(s.Href) &&
                        !s.Href.EndsWith("/layer.json", StringComparison.OrdinalIgnoreCase))
                    {
                        s.Href = s.Href.TrimEnd('/') + "/layer.json";
                    }

                    // imagery: {z}/{x}/{y} -> {z}/{x}/{ReverseY}
                    if (t == BaseLayerType.Imagery && !string.IsNullOrWhiteSpace(s.Href) &&
                        s.Href.Contains("{z}/{x}/{y}"))
                    {
                        s.Href = s.Href.Replace("{z}/{x}/{y}", "{z}/{x}/{ReverseY}");
                    }

                    var layer = new BaseLayerCombined
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Href = s.Href,
                        Type = t,
                        Description = s.Description,
                        IsVisible = true, // when no project is set: visible
                        IsDownloaded = false,
                        IsRestricted = false
                    };

                    if (!_byId.ContainsKey(layer.Id))
                    {
                        _byId[layer.Id] = layer;
                        _combinedLayers.Add(layer);
                    }
                }
            }
            else
            {
                // No server response: use downloaded layers from disk (both public and restricted)
                foreach (var layer in DiscoverDownloadedLayersFromDisk())
                {
                    if (layer == null || string.IsNullOrWhiteSpace(layer.Id)) continue;
                    if (_byId.ContainsKey(layer.Id)) continue;
                    _byId[layer.Id] = layer;
                    _combinedLayers.Add(layer);
                }

                if (_combinedLayers.Count == 0)
                    ULog.Warning("No base layers available (server empty and no downloaded layers found).");
            }

            await DestroyAllInstancesAsync();
            await InstantiateAllBaseLayersAsync();
            await _downloader.RefreshDownloadedStatusAsync(_combinedLayers);
            _instancesInitialized = true;

            try
            {
                _progressProvider?.StartLoadingProgressCalculation();
            }
            catch
            {
            }

            RaiseCombinedChanged();
        }

        private IEnumerable<BaseLayerCombined> DiscoverDownloadedLayersFromDisk()
        {
            var list = new List<BaseLayerCombined>();
            if (_downloader == null)
            {
                ULog.Info("DiscoverDownloadedLayersFromDisk: _downloader not initialized. Returning no local layers.");
                return list;
            }
            var root = _downloader.DownloadRoot;

            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root)) return list;

            foreach (var dir in Directory.EnumerateDirectories(root))
            {
                var id = Path.GetFileName(dir);
                var manifest = Path.Combine(dir, "manifest.json");
                if (!File.Exists(manifest)) continue;

                try
                {
                    var text = File.ReadAllText(manifest);
                    var obj = JsonConvert.DeserializeObject<JObject>(text);
                    var href = obj?["href"]?.Value<string>();
                    var typeStr = obj?["type"]?.Value<string>() ?? obj?["Type"]?.Value<string>();
                    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(typeStr)) continue;
                    if (!TryMapType(typeStr, out var t)) continue;
                    var name = obj?["name"]?.Value<string>();
                    if (string.IsNullOrWhiteSpace(name)) name = id;
                    var description = obj?["description"]?.Value<string>() ?? string.Empty;

                    // read persisted access
                    var access = obj?["access"]?.Value<string>() ?? obj?["Access"]?.Value<string>();
                    var isRestricted = string.Equals(access, "RESTRICTED", StringComparison.OrdinalIgnoreCase);

                    list.Add(new BaseLayerCombined
                    {
                        Id = id,
                        Name = name,
                        Href = href,
                        Type = t,
                        Description = description,
                        // default visibility when no project: public true; restricted uses flag
                        IsVisible = isRestricted ? _restrictedVisibleByDefault : true,
                        IsDownloaded = true,
                        IsRestricted = isRestricted
                    });
                }
                catch
                {
                    /* ignore broken manifests */
                }
            }

            return list;
        }

        /// <summary>
        /// Buld delete all restricted layers that are downloaded.
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> DeleteAllRestrictedAsync()
        {
            bool any = false;
            foreach (var layer in _combinedLayers.Where(x => x.IsRestricted).ToList())
            {
                try
                {
                    var ok = await _downloader.DeleteLayerAsync(layer);
                    any |= ok;
                    ApplyLayerUrl(layer); // will clear local url if any
                    ApplyLayerVisibility(layer);
                }
                catch (Exception e)
                {
                    ULog.Warning($"DeleteAllRestrictedAsync: {e.Message}");
                }
            }
            if (any)
            {
                await _downloader.RefreshDownloadedStatusAsync(_combinedLayers);
                RaiseCombinedChanged();
            }
            return any;
        }

        private static bool TryMapType(string typeStr, out BaseLayerType t)
        {
            switch (typeStr?.Trim()?.ToUpperInvariant())
            {
                case "TILES3D": t = BaseLayerType.Tiles3D; return true;
                case "IMAGERY": t = BaseLayerType.Imagery; return true;
                case "TERRAIN": t = BaseLayerType.Terrain; return true;
                default: t = BaseLayerType.Tiles3D; return false;
            }
        }

        

        //  Scene / runtime 

        private void RebuildIdIndex()
        {
            _byId.Clear();
            foreach (var c in _combinedLayers)
            {
                if (!string.IsNullOrWhiteSpace(c.Id))
                    _byId[c.Id] = c;
            }
        }

        private async UniTask RebuildSceneForCombinedAsync(ProjectVariant variant, int version)
        {
            if (!_instancesInitialized)
            {
                await DestroyAllInstancesAsync();
                await InstantiateAllBaseLayersAsync();
                _instancesInitialized = true;
                await _downloader.RefreshDownloadedStatusAsync(_combinedLayers);
            }
            else
            {
                foreach (var l in _combinedLayers)
                {
                    ApplyLayerUrl(l); // picks local URL if downloaded
                    ApplyLayerVisibility(l); // toggle GO / behaviour accordingly
                }
                SyncImageryOverlayToCurrentState(); 
            }

            ClearVariantPolygons();
            ClearVariantModels();

            if (variant != null)
            {
                CreateAndWireClippingPolygons(variant);
                await InstantiateModelsFromVariantAsync(variant);
            }

            //try { _progressProvider?.StartLoadingProgressCalculation(); } catch { }
        }


        private async UniTask InstantiateAllBaseLayersAsync()
        {
            if (_geoRefParent == null)
            {
                ULog.Warning("GeoRef parent missing. Skipping instantiation.");
                return;
            }

            BaseLayerCombined primaryTerrain = null;
            
            //Instantiate TERRAIN and TILES3D gameobjects

            foreach (var l in _combinedLayers.Where(x => x.Type != BaseLayerType.Imagery))
            {
                // Instantiate then toggle visibility state
                var template = SelectTemplate(l.Type, l.Href);
                var go = SafeInstantiate(template, _geoRefParent, $"{l.Name}");
                l.Instance = go;

                if (l.Type == BaseLayerType.Terrain)
                {
                    var tileset = FindCesiumTileset(go);
                    if (tileset == null) ULog.Warning($"Terrain template missing Cesium3DTileset for '{l.Name}'.");
                    else
                    {
                        SetTilesetUrl(tileset, l.Href);
                        l.BoundComponent = tileset;
                        primaryTerrain ??= l;
                    }

                    // Host for polygons on terrain
                    l.OverlayForPolygons = EnsurePolygonOverlay(go);
                }
                else // Tiles3D
                {
                    var tileset = FindCesiumTileset(go);
                    if (tileset == null) ULog.Warning($"Tiles3D template missing Cesium3DTileset for '{l.Name}'.");
                    else
                    {
                        SetTilesetUrl(tileset, l.Href);
                        l.BoundComponent = tileset;
                    }
                    l.OverlayForPolygons = EnsurePolygonOverlay(go);
                }

                ApplyLayerUrl(l);
                ApplyLayerVisibility(l);
                SyncImageryOverlayToCurrentState();
                await UniTask.Yield();
            }
            if (primaryTerrain == null || primaryTerrain.Instance == null)
            {
                ULog.Warning("No terrain instance available to host imagery overlays. Imagery layers will be skipped.");
                return;
            }

            var terrainOverlay = GetExistingUrlTemplateOverlay(primaryTerrain.Instance);
            if (terrainOverlay == null)
            {
                ULog.Warning("Terrain template has no CesiumUrlTemplateRasterOverlay component.");
                return;
            }
            // Choose one active imagery (first visible); single overlay can show only one at a time
            BaseLayerCombined activeImagery = _combinedLayers
                .FirstOrDefault(x => x.Type == BaseLayerType.Imagery && x.IsVisible);

            foreach (var l in _combinedLayers.Where(x => x.Type == BaseLayerType.Imagery))
            {
                // Bind imagery layer to the ONE existing overlay on terrain
                l.Instance = primaryTerrain.Instance;
                l.BoundComponent = terrainOverlay;
                l.OverlayForPolygons = null; // polygons don’t attach to imagery

                if (l == activeImagery)
                {
                    ApplyLayerUrl(l);  // sets overlay.templateUrl to this layer’s URL (or local)
                    if (terrainOverlay is Behaviour ob) ob.enabled = true;
                }
                else
                {
                    // Keep them logically present; visibility handled by which URL is set.
                    l.IsVisible = false;
                }

                await UniTask.Yield();
            }
        }
        
        private void SetOverlayTemplate(Component overlay, string templateOrEmpty)
        {
            if (overlay == null) return;
            SetOverlayUrl(overlay, templateOrEmpty ?? string.Empty);
            TryRecreateTileset(overlay);
        }

        private Component GetExistingUrlTemplateOverlay(GameObject host)
        {
            if (host == null) return null;
            // Do NOT create new; just return what's already on the template
            var overlay = host.GetComponent("CesiumUrlTemplateRasterOverlay");
            if (overlay is Behaviour b) b.enabled = true;
            TryInvokeAddToTileset(overlay);
            return overlay;
        }
        
        private void ApplyLayerVisibility(BaseLayerCombined layer)
        {
            if (layer == null) return;

            if (layer.Type == BaseLayerType.Imagery)
            {
                // Imagery handled via single overlay elsewhere.
                return;
            }

            // Instantiate on-demand for layers that were merged later (e.g., restricted)
            if (layer.IsVisible && layer.Instance == null)
            {
                if (_geoRefParent == null)
                {
                    ULog.Warning("GeoRef parent missing. Cannot instantiate layer on-demand.");
                }
                else
                {
                    var template = SelectTemplate(layer.Type, layer.Href);
                    var go = SafeInstantiate(template, _geoRefParent, $"{layer.Name}");
                    layer.Instance = go;

                    var tileset = FindCesiumTileset(go);
                    if (tileset == null)
                    {
                        ULog.Warning($"On-demand instantiation missing Cesium3DTileset for '{layer.Name}'.");
                    }
                    else
                    {
                        SetTilesetUrl(tileset, layer.Href);
                        layer.BoundComponent = tileset;
                    }
                    layer.OverlayForPolygons = EnsurePolygonOverlay(go);

                    // Ensure URL honors local cache if available
                    ApplyLayerUrl(layer);
                }
            }

            if (layer.Instance != null) layer.Instance.SetActive(layer.IsVisible);
            if (layer.BoundComponent is Behaviour b) b.enabled = layer.IsVisible;
        }

        private void ApplyLayerUrl(BaseLayerCombined layer)
        {
            if (layer == null) return;

            var local = _downloader.GetLocalUrlIfDownloaded(layer);
            var urlToUse = string.IsNullOrEmpty(local) ? layer.Href : local;

            if (layer.Type == BaseLayerType.Imagery)
            {
                if (layer.BoundComponent != null)
                {
                    SetOverlayUrl(layer.BoundComponent, urlToUse);
                    TryRecreateTileset(layer.BoundComponent); // refresh tileset so overlay refetches
                }
                return;
            }

            if (layer.BoundComponent != null) // Terrain / Tiles3D
            {
                SetTilesetUrl(layer.BoundComponent, urlToUse);
                TryRecreateTileset(layer.BoundComponent);
            }
        }

        private void ApplyClippingPolygonsToMatchingLayers(ProjectVariant variant)
        {
            if (variant?.ClippingPolygons == null || variant.ClippingPolygons.Count == 0) return;

            // For every visible base layer, attach polygons according to its type and AffectsTerrain
            foreach (var layer in _combinedLayers)
            {
                if (!layer.IsVisible || layer.Instance == null) continue;

                // Polygons apply to tilesets (Terrain + Tiles3D) only; skip Imagery.
                var isTilesetLayer = layer.Type == BaseLayerType.Terrain ||
                                     layer.Type == BaseLayerType.Tiles3D;
                if (!isTilesetLayer) continue;

                // Must have a polygon overlay host on this layer's GO
                if (layer.OverlayForPolygons == null)
                {
                    // Try to ensure one now (defensive); if still null, skip
                    layer.OverlayForPolygons = EnsurePolygonOverlay(layer.Instance);
                    if (layer.OverlayForPolygons == null) continue;
                }

                foreach (var cp in variant.ClippingPolygons)
                {
                    if (cp == null || cp.Points == null || cp.Points.Count < 3) continue;

                    // Respect AffectsTerrain:
                    // - If polygon does NOT affect terrain, skip terrain layers.
                    // - Tiles3D always gets polygons.
                    if (layer.Type == BaseLayerType.Terrain && !cp.AffectsTerrain) continue;

                    var go = new GameObject($"Polygon_{cp.Id}_L_{layer.Id}");
                    go.transform.SetParent(layer.Instance.transform, false);
                    
                    var comp = go.AddComponent<CesiumCartographicPolygon>();
                    if (comp == null)
                    {
                        ULog.Warning("CesiumCartographicPolygon type not found. Skipping polygon.");
                        Destroy(go);
                        continue;
                    }

                    try
                    {
                        var method = comp.GetType().GetMethod("SetPositionsUnity",
                            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        method?.Invoke(comp, new object[] { cp.Points });
                    }
                    catch (Exception e)
                    {
                        ULog.Warning($"Failed to assign polygon points: {e.Message}");
                    }

                    _activeVariantPolygons.Add(comp);
                }
            }
        }

        private async UniTask InstantiateModelsFromVariantAsync(ProjectVariant variant)
        {
            if (_geoRefParent == null || variant?.Models == null) return;

            foreach (var m in variant.Models)
            {
                if (m == null || string.IsNullOrWhiteSpace(m.Url)) continue;

                var ecefTest = new Vector3d(m.Translation.x, m.Translation.y, m.Translation.z);
                if (!IsValidEcef(ecefTest))
                {
                    ULog.Warning($"Skipping model '{m.Id}': invalid ECEF ({ecefTest.X}, {ecefTest.Y}, {ecefTest.Z}).");
                    continue; // do not instantiate, do not track
                }

                var go = new GameObject($"Model_{m.Id}");
                go.transform.SetParent(_geoRefParent, false);
                go.layer = LayerMask.NameToLayer("City");
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                //go.transform.localRotation = Quaternion.AngleAxis(-90f, Vector3.right);
                
                var modelAttributes = go.AddComponent<ProjectModelAttributes>();
                modelAttributes.Initialize(m.Id, m.Attributes);

                // Place via GlobeAnchor (ECEF + EUN)
                var ecef = new Vector3d(m.Translation.x, m.Translation.y, m.Translation.z);
                //var rotEcef  = SanitizeQuaternion(m.Rotation);
                var rotEcef  = m.Rotation;
                var scaleEun = SanitizeScale(m.Scale);

                CesiumPlacement.AnchorAtEcef(go, ecef, rotEcef, scaleEun);
                //go.transform.localRotation = Quaternion.AngleAxis(-90f, Vector3.right);
                // Load GLB
                //var asset = go.AddComponent<GltfAsset>();
                var asset = go.AddComponent<GltfBoundsAsset>();
                asset.LoadOnStartup = true;
                asset.CreateBoxCollider = true;
                //asset.StreamingAsset = false;
                asset.InstantiationSettings = new InstantiationSettings();
                asset.InstantiationSettings.SkinUpdateWhenOffscreen = false;
                asset.InstantiationSettings.Layer = LayerMask.NameToLayer("City"); //LayerMask.GetMask("City");
                asset.InstantiationSettings.Mask = ComponentType.Mesh;
                
                var url = m.Url;
                if (IsHttpOrHttps(url))
                {
                    url = AppendQueryIfAny(url, CurrentProject?.ProjectSasQueryParameters);
                }
                if (!IsHttpOrHttps(url) && !url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    // keep existing file:// fallback for non-http inputs
                    url = new Uri(Path.GetFullPath(url)).AbsoluteUri;
                }
                //if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                //    url = new Uri(System.IO.Path.GetFullPath(url)).AbsoluteUri;
                
                asset.Url = url;

                _activeVariantModels.Add(go);

                // fix for either server or gltf rotation mismatch
                await UniTask.WaitForEndOfFrame();
                var anchor = go.GetComponent<CesiumGlobeAnchor>();
                anchor.rotationEastUpNorth = anchor.rotationEastUpNorth * _axisFix;
                //anchor.rotationEastUpNorth = anchor.rotationEastUpNorth * Quaternion.AngleAxis(-90f, Vector3.right);
                //anchor.rotationEastUpNorth = anchor.rotationEastUpNorth * Quaternion.AngleAxis(-90f, Vector3.up);
                await UniTask.WaitForEndOfFrame();
                go.AddComponent<CustomAttributeXRHandler>();
            }
        }

        private static readonly Quaternion _axisFix = Quaternion.AngleAxis(-90f, Vector3.right) * Quaternion.AngleAxis(-90f, Vector3.up);

        private static bool IsHttpOrHttps(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        private static string AppendQueryIfAny(string url, string extraQuery)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(extraQuery)) return url;

            // strip any leading '?' or '&' from the provided SAS string
            var cleaned = extraQuery.TrimStart('?', '&');

            // if nothing remains, return original
            if (string.IsNullOrWhiteSpace(cleaned)) return url;

            return url.Contains("?")
                ? $"{url}&{cleaned}"
                : $"{url}?{cleaned}";
        }

        private void CreateAndWireClippingPolygons(ProjectVariant variant)
        {
            if (variant?.ClippingPolygons == null || variant.ClippingPolygons.Count == 0) return;
            if (_geoRefParent == null) return;

            // get CesiumGeoreference from the CesiumGeoRef object
            var geoRefGo = GameObject.FindGameObjectWithTag("CesiumGeoRef"); // existing scene contract
            var geoRef = geoRefGo != null ? geoRefGo.GetComponent<CesiumForUnity.CesiumGeoreference>() : null;
            if (geoRef == null)
            {
                ULog.Warning("CesiumGeoreference not found on CesiumGeoRef. Skipping polygon creation.");
                return;
            }

            // collect overlays for all non-imagery layers (Terrain + Tiles3D)
            var overlays = _combinedLayers
                .Where(l => l.Type != BaseLayerType.Imagery && l.Instance != null)
                .Select(l => new
                {
                    Layer = l,
                    Overlay =
                        EnsurePolygonOverlay(l.Instance) as
                            CesiumForUnity.CesiumPolygonRasterOverlay // expect concrete type
                })
                .Where(x => x.Overlay != null)
                .ToList();

            foreach (var cp in variant.ClippingPolygons)
            {
                if (cp == null || cp.Points == null || cp.Points.Count < 3) continue;

                // create a polygon GameObject under CesiumGeoRef with identity transform
                var polyGo = new GameObject($"Polygon_{cp.Id}");
                polyGo.transform.SetParent(_geoRefParent, false);
                polyGo.transform.localPosition = Vector3.zero;
                polyGo.transform.localRotation = Quaternion.identity;
                polyGo.transform.localScale = Vector3.one;

                // required components (match official CesiumCartographicPolygon expectations)
                var splineContainer = polyGo.AddComponent<UnityEngine.Splines.SplineContainer>();
                var globeAnchor = polyGo.AddComponent<CesiumForUnity.CesiumGlobeAnchor>();
                var originShift = polyGo.AddComponent<CesiumForUnity.CesiumOriginShift>();
                var cartoPoly = polyGo.AddComponent<CesiumForUnity.CesiumCartographicPolygon>();

                // convert each server ECEF point -> Unity world 
                var unityWorld = new List<Unity.Mathematics.float3>(cp.Points.Count);
                for (int i = 0; i < cp.Points.Count; i++)
                {
                    var p = cp.Points[i];
                    var unity = geoRef.TransformEarthCenteredEarthFixedPositionToUnity(
                        new Unity.Mathematics.double3((double)p.x, (double)p.y,
                            (double)p.z)); 
                    unityWorld.Add((Unity.Mathematics.float3)unity);
                }
                
                //clear default spline on component
                var existing = splineContainer.Splines;
                for (int i = existing.Count - 1; i >= 0; i--)
                    splineContainer.RemoveSpline(existing[i]);

                // author one linear, closed spline at those positions (local == world because GO is identity)
                var spline = new Spline();
                var knots = new BezierKnot[unityWorld.Count];
                for (int i = 0; i < unityWorld.Count; i++)
                    knots[i] = new BezierKnot(unityWorld[i]);
                spline.Knots = knots; 
                spline.Closed = true;
                spline.SetTangentMode(TangentMode.Linear);
                splineContainer.AddSpline(spline);

                _activeVariantPolygons.Add(cartoPoly); // keep for unload/cleanup

                // wire this polygon into every relevant overlay via the public API
                foreach (var x in overlays)
                {
                    // Respect AffectsTerrain: skip terrain overlays if false
                    if (x.Layer.Type == BaseLayerType.Terrain && !cp.AffectsTerrain) continue;

                    var overlayCmp = x.Overlay;
                    if (overlayCmp == null) continue;

                    if (overlayCmp.polygons == null)
                        overlayCmp.polygons =
                            new System.Collections.Generic.List<CesiumForUnity.CesiumCartographicPolygon>();

                    if (!overlayCmp.polygons.Contains(cartoPoly))
                        overlayCmp.polygons.Add(cartoPoly);

                    // Ensure overlay is active and re-applied to its tileset
                    if (overlayCmp is Behaviour b) b.enabled = true; 
                    overlayCmp.Refresh();
                }
            }
        }


        private static (double lonDeg, double latDeg, double height) EcefToCartographic(double x, double y, double z)
        {
            // Standard iterative WGS-84 conversion (sufficient for runtime)
            const double a = 6378137.0;
            const double f = 1.0 / 298.257223563;
            double b = a * (1.0 - f);
            double e2 = 1.0 - (b * b) / (a * a);

            double p = Math.Sqrt(x * x + y * y);
            double theta = Math.Atan2(z * a, p * b);
            double sinTheta = Math.Sin(theta);
            double cosTheta = Math.Cos(theta);

            double lat = Math.Atan2(z + (e2 * b) * sinTheta * sinTheta * sinTheta,
                p - (e2 * a) * cosTheta * cosTheta * cosTheta);
            double lon = Math.Atan2(y, x);

            double sinLat = Math.Sin(lat);
            double N = a / Math.Sqrt(1.0 - e2 * sinLat * sinLat);
            double h = p / Math.Cos(lat) - N;

            return (lon * Mathf.Rad2Deg, lat * Mathf.Rad2Deg, h);
        }


        private async UniTask DestroyAllInstancesAsync()
        {
            foreach (var l in _combinedLayers)
            {
                if (l.Instance != null)
                {
                    try { Destroy(l.Instance); } catch { }
                    l.Instance = null;
                    l.BoundComponent = null;
                    l.OverlayForPolygons = null;
                }
            }
            await UniTask.WaitForEndOfFrame();
        }

        private void ClearVariantPolygons()
        {
            foreach (var c in _activeVariantPolygons)
            {
                if (c != null) try { Destroy(c.gameObject); } catch { }
            }
            _activeVariantPolygons.Clear();
        }

        private void ClearVariantModels()
        {
            foreach (var go in _activeVariantModels)
            {
                if (go != null) try { Destroy(go); } catch { }
            }
            _activeVariantModels.Clear();
        }

        //  Cesium helpers  

        private GameObject SelectTemplate(BaseLayerType type, string href)
        {
            if (type == BaseLayerType.Terrain) return _cesiumTerrainTemplateInScene;
            if (type == BaseLayerType.Imagery) return _cesiumTerrainTemplateInScene; // imagery needs a tileset host; we instantiate its own GO for independence
            // Tiles3D
            return IsTreesHref(href) ? (_cesiumTreesTemplateInScene ?? _cesiumCityTemplateInScene) : _cesiumCityTemplateInScene;
        }

        private GameObject SafeInstantiate(GameObject template, Transform parent, string nameOverride)
        {
            if (template == null)
            {
                ULog.Warning("Template not assigned in Inspector.");
                return null;
            }
            var inst = Instantiate(template, parent, false);
            inst.name = nameOverride;
            inst.SetActive(true);
            return inst;
        }

        private Component FindCesiumTileset(GameObject go)
        {
            if (go == null) return null;
            var comp = go.GetComponent("Cesium3DTileset");
            return comp;
        }

        private void SetTilesetUrl(Component tileset, string url)
        {
            if (tileset == null || string.IsNullOrWhiteSpace(url)) return;
            var t = tileset.GetType();
            var tilesetSourceProp = t.GetProperty("tilesetSource");
            var urlProp = t.GetProperty("url");
            if (tilesetSourceProp != null)
            {
                var enumType = tilesetSourceProp.PropertyType;
                try
                {
                    var fromUrl = Enum.Parse(enumType, "FromUrl");
                    tilesetSourceProp.SetValue(tileset, fromUrl);
                }
                catch { }
            }
            urlProp?.SetValue(tileset, url);
        }

        private Component EnsureUrlTemplateOverlay(GameObject go)
        {
            if (go == null) return null;
            var overlay = go.GetComponent<CesiumUrlTemplateRasterOverlay>();
            
            if (overlay is Behaviour b) b.enabled = true;
            TryInvokeAddToTileset(overlay);
            return overlay;
        }
        
        private Component EnsurePolygonOverlay(GameObject go)
        {
            if (go == null) return null;
            var overlay = go.GetComponent<CesiumPolygonRasterOverlay>();
           
            if (overlay is Behaviour b) b.enabled = true;
            TryInvokeAddToTileset(overlay);
            return overlay;
        }

        private static bool IsFinite(float v) => !float.IsNaN(v) && !float.IsInfinity(v);

        private static bool IsValidEcef(FHH.Logic.Models.Vector3d ecef)
        {
            if (ecef == null) return false;
            double x = ecef.X, y = ecef.Y, z = ecef.Z;
            if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z)) return false;
            if (double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(z)) return false;

            // Reject near-zero length vectors (undefined ENU at Earth center)
            // Threshold ~1 meter to be safe against tiny placeholders.
            double magSq = x * x + y * y + z * z;
            return magSq > 1.0;
        }

        //  sanitize a Quaternion coming from the server (ECEF frame)
        private static Quaternion SanitizeQuaternion(Quaternion q)
        {
            if (float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w)) return Quaternion.identity;
            float len = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (len < 1e-6f) return Quaternion.identity;
            return new Quaternion(q.x / len, q.y / len, q.z / len, q.w / len);
        }

        // sanitize an EUN scale vector
        private static Vector3 SanitizeScale(Vector3 s)
        {
            if (!IsFinite(s.x) || !IsFinite(s.y) || !IsFinite(s.z)) return Vector3.one;
            if (Mathf.Abs(s.x) < 1e-9f) s.x = 1f;
            if (Mathf.Abs(s.y) < 1e-9f) s.y = 1f;
            if (Mathf.Abs(s.z) < 1e-9f) s.z = 1f;
            return s;
        }
        
        private void SetOverlayUrl(Component overlay, string templateUrl)
        {
            if (overlay == null || string.IsNullOrWhiteSpace(templateUrl)) return;
            var t = overlay.GetType();
            var prop = t.GetProperty("templateUrl");
            if (prop != null) prop.SetValue(overlay, templateUrl);
            else
            {
                var urlProp = t.GetProperty("url");
                if (urlProp != null) urlProp.SetValue(overlay, templateUrl);
                else
                {
                    var field = t.GetField("_templateUrl", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null) field.SetValue(overlay, templateUrl);
                    else ULog.Warning("Raster overlay does not expose templateUrl/url. URL not set.");
                }
            }
            TryInvokeAddToTileset(overlay);
        }

        private void SyncImageryOverlayToCurrentState()
        {
            var terrain = _combinedLayers.FirstOrDefault(x => x.Type == BaseLayerType.Terrain && x.Instance != null);
            if (terrain == null) return;
            var overlay = GetExistingUrlTemplateOverlay(terrain.Instance);
            if (overlay == null) return;

            var active = _combinedLayers.FirstOrDefault(x => x.Type == BaseLayerType.Imagery && x.IsVisible);
            if (active != null)
            {
                var local = _downloader.GetLocalUrlIfDownloaded(active);
                var url = string.IsNullOrEmpty(local) ? active.Href : local;
                SetOverlayTemplate(overlay, url);
                if (overlay is Behaviour ob) ob.enabled = true;
            }
            else
            {
                SetOverlayTemplate(overlay, string.Empty);
                if (overlay is Behaviour ob) ob.enabled = false;
            }
        }


        private void TryInvokeAddToTileset(Component overlay)
        {
            try
            {
                var m = overlay.GetType().GetMethod("AddToTileset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                m?.Invoke(overlay, null);
            }
            catch (Exception e)
            {
                ULog.Warning($"AddToTileset() call failed on overlay: {e.Message}");
            }
        }

        private void TryRecreateTileset(Component tileset)
        {
            try
            {
                var m = tileset?.GetType().GetMethod("RecreateTileset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                m?.Invoke(tileset, null);
            }
            catch { }
        }

        private static bool IsTreesHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return false;
            return href.IndexOf("tree", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   href.IndexOf("trees", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RaiseCombinedChanged() => OnCombinedLayersChanged?.Invoke(_combinedLayers);

        public async UniTask<Project> GetProjectAsync(string id)
        {
            try
            {
                var projectJson = await GetDataFromEndpointAsync<string>(_appConfig.ServerConfig.Endpoints.First(e => e.Name == "Project").Path.Replace("{id}", id));
                var project = Project.FromJson(projectJson);
                return project;
            }
            catch (Exception ex)
            {
                ULog.Error("Fetching Project failed: " + ex.Message);
                return null;
            }
        }

        
        private async UniTask<T> GetDataFromPublicEndpointAsync<T>(string endpoint)
        {
            if (ServiceLocator.Instance.IsOffline || _http == null || _appConfig == null)
            {
                ULog.Info($"GetDataFromPublicEndpointAsync: offline or HTTP/config not available, skipping '{endpoint}'.");
                return await UniTask.FromResult(default(T));
            }
            var baseUrl = _appConfig.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}/{endpoint}";
            
            var headers = new Dictionary<string, string> { ["Accept"] = "application/json" };
            try
            {
                var httpResponse = await _http.GetAsync(url, headers);
                if (!httpResponse.IsSuccess)
                {
                    ULog.Warning($"GET {endpoint} failed: {httpResponse.StatusCode} - {httpResponse.ErrorMessage}");
                    return await UniTask.FromResult(default(T));
                }
                return JsonConvert.DeserializeObject<T>(httpResponse.Data);
            }
            catch (Exception e)
            {
                ULog.Warning($"HTTP error for '{endpoint}': {e.Message}");
                return await UniTask.FromResult(default(T));
            }
        }

        private async UniTask<T> GetDataFromEndpointAsync<T>(string endpoint)
        {
            if (ServiceLocator.Instance.IsOffline || _http == null || _appConfig == null)
            {
                ULog.Info($"GetDataFromEndpointAsync: offline or HTTP/config not available, skipping '{endpoint}'.");
                return await UniTask.FromResult(default(T));
            }
            var tokenProvider = _auth.GetTokenStorageProvider();
            var accessToken = tokenProvider.GetAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                ULog.Warning("No access token found.");
                return await UniTask.FromResult(default(T));
            }
            var baseUrl = _appConfig.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}/{endpoint}";
            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {accessToken}",
                ["Accept"] = "application/json"
            };
            var httpResponse = await _http.GetAsync(url, headers);
            if (!httpResponse.IsSuccess)
            {
                ULog.Warning($"Failed to fetch data from endpoint '{endpoint}'. Status: {httpResponse.StatusCode}. Error: {httpResponse.ErrorMessage}");
                return await UniTask.FromResult(default(T));
            }
            if (typeof(T) == typeof(string))
            {
                return (T)(object)httpResponse.Data;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(httpResponse.Data);
            }
            catch (Exception ex)
            {
                ULog.Error($"Deserialization to {typeof(T).Name} failed: {ex.Message}");
                return await UniTask.FromResult(default(T));
            }
        }

        private async void OnUserChangedPermissionService(object sender, PermissionService.UserChangedEventArgs args)
        {
            try
            {
                var perm = ServiceLocator.GetService<PermissionService>();
                if (perm != null && !perm.IsAnonymous())
                {
                    await LoadRestrictedLayersAsync();
                }
            }
            catch (Exception e)
            {
                ULog.Info($"OnUserChangedPermissionService: {e.Message}");
            }
        }

        public async UniTask<bool> RefreshRestrictedAsync()
        {
            return await LoadRestrictedLayersAsync();
        }

        
        private async UniTask<bool> LoadRestrictedLayersAsync()
        {
            int added = 0;

            // DISK: merge any restricted layers already downloaded
            try
            {
                var discovered = DiscoverDownloadedLayersFromDisk();
                foreach (var d in discovered)
                {
                    if (d == null || !d.IsRestricted || string.IsNullOrWhiteSpace(d.Id))
                        continue;

                    // Public wins on duplicate Ids; if an entry already exists, keep it (preserve visibility/instances)
                    if (_byId.TryGetValue(d.Id, out var existing))
                    {
                        // If the existing is public, skip disk restricted
                        if (existing != null && !existing.IsRestricted) continue;

                        // If existing is restricted, keep existing; optionally refresh non-critical metadata
                        if (existing != null)
                        {
                            if (!string.IsNullOrWhiteSpace(d.Name)) existing.Name = existing.Name ?? d.Name;
                            if (!string.IsNullOrWhiteSpace(d.Description))
                                existing.Description = existing.Description ?? d.Description;
                        }

                        continue;
                    }

                    // Insert as metadata-only: invisible, no instances; downloaded flags already true from manifest
                    d.IsVisible = false;
                    d.Instance = null;
                    d.BoundComponent = null;
                    d.OverlayForPolygons = null;

                    _combinedLayers.Add(d);
                    _byId[d.Id] = d;
                    added++;
                }
            }
            catch (Exception ex)
            {
                ULog.Info($"LoadRestrictedLayersAsync (disk) skipped/failed: {ex.Message}");
            }

            // SERVER: if authenticated, fetch restricted list and merge additions
            try
            {
                var perm = ServiceLocator.GetService<PermissionService>();
                if (perm != null && !perm.IsAnonymous())
                {
                    var restrictedEndpoint = _appConfig.ServerConfig.EndpointPath("BaseLayersRestricted");
                    var serverRestricted = await GetDataFromEndpointAsync<List<ServerBaseLayerDto>>(restrictedEndpoint);

                    if (serverRestricted != null && serverRestricted.Count > 0)
                    {
                        foreach (var s in serverRestricted)
                        {
                            if (s == null || string.IsNullOrWhiteSpace(s.Id) || string.IsNullOrWhiteSpace(s.Type))
                                continue;
                            if (!TryMapType(s.Type, out var t)) continue;

                            // Normalize
                            if (t == BaseLayerType.Terrain && !string.IsNullOrWhiteSpace(s.Href) &&
                                !s.Href.EndsWith("/layer.json", StringComparison.OrdinalIgnoreCase))
                                s.Href = s.Href.TrimEnd('/') + "/layer.json";

                            if (t == BaseLayerType.Imagery && !string.IsNullOrWhiteSpace(s.Href) &&
                                s.Href.Contains("{z}/{x}/{y}"))
                                s.Href = s.Href.Replace("{z}/{x}/{y}", "{z}/{x}/{ReverseY}");

                            if (_byId.TryGetValue(s.Id, out var existing))
                            {
                                // Public wins; skip if existing is public
                                if (existing != null && !existing.IsRestricted) continue;

                                // Existing restricted: optionally refresh non-critical metadata, never flip visibility/instances here

                                if (!string.IsNullOrWhiteSpace(s.Href)) existing.Href ??= s.Href;
                                if (existing != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(s.Name)) existing.Name ??= s.Name;
                                    if (!string.IsNullOrWhiteSpace(s.Description))
                                        existing.Description ??= s.Description;
                                    // Keep existing.Href if set; runtime will prefer local if downloaded anyway
                                }

                                continue;
                            }

                            var layer = new BaseLayerCombined
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Href = s.Href,
                                Type = t,
                                Description = s.Description,
                                IsVisible = false, // metadata only, user/project decides later
                                IsDownloaded = false,
                                IsRestricted = true,
                                Instance = null,
                                BoundComponent = null,
                                OverlayForPolygons = null
                            };

                            _combinedLayers.Add(layer);
                            _byId[layer.Id] = layer;
                            added++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ULog.Info($"LoadRestrictedLayersAsync (server) skipped/failed: {ex.Message}");
            }

            // Post-merge: refresh downloaded flags (cheap) and notify
            if (added > 0 || !_restrictedLoadedOnce)
            {
                await _downloader.RefreshDownloadedStatusAsync(_combinedLayers);
                RaiseCombinedChanged();
            }

            _restrictedLoadedOnce = true;
            return added > 0;
        }

        public async UniTask ResetToGeneralLayersOnlyAsync()
        {
            var restricted = _combinedLayers
                .Where(l => l.IsRestricted)
                .ToList();

            foreach (var layer in restricted)
            {
                if (layer.Instance != null)
                {
                    try
                    {
                        Destroy(layer.Instance);
                    }
                    catch
                    {
                    }

                    layer.Instance = null;
                    layer.BoundComponent = null;
                    layer.OverlayForPolygons = null;
                }

                if (!string.IsNullOrWhiteSpace(layer.Id))
                {
                    _byId.Remove(layer.Id);
                }

                _combinedLayers.Remove(layer);
            }

            SyncImageryOverlayToCurrentState();
            RaiseCombinedChanged();

            await UniTask.WaitForEndOfFrame();
        }
    }
}