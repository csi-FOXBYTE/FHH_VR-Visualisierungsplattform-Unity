using Cysharp.Threading.Tasks;
using FHH.Logic;
using FHH.Logic.Models;
using Foxbyte.Core;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Presentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FHH.UI.ProjectLibrary
{
    public sealed class ProjectLibraryItem
    {
        public string Id;
        public string Name;
        public string Description;
        //public string DownloadUrl;
    }

    public sealed class ProjectLibraryItems : List<ProjectLibraryItem>
    {
        public static ProjectLibraryItems FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("json is null or empty", nameof(json));

            var items = JsonConvert.DeserializeObject<List<ProjectLibraryItem>>(json);
            var result = new ProjectLibraryItems();
            if (items != null) result.AddRange(items);
            return result;
        }
    }

    public enum ProjectLibraryTab
    {
        Mine,
        SharedWithMe
    }

    public sealed class ProjectLibraryModel : PresenterModelBase
    {
        public string ActiveProjectId { get; private set; }
        public event Action<string> ActiveProjectIdChanged;
        private readonly List<ProjectLibraryItem> _mine = new();
        private readonly List<ProjectLibraryItem> _shared = new();
        private IAuthenticationService _auth;
        private IHttpClient _http;
        private AppConfig _appConfig;
        private ProjectDownloader _downloader;
        public ProjectDownloader Downloader => _downloader;

        public IReadOnlyList<ProjectLibraryItem> Current => _current;
        private List<ProjectLibraryItem> _current = new();

        public ProjectLibraryTab CurrentTab { get; private set; } = ProjectLibraryTab.Mine;

        public override async UniTask InitializeAsync()
        {
            _auth = ServiceLocator.GetService<OAuth2AuthenticationService>();
            _http = new HttpClientWithRetry();
            _appConfig = ServiceLocator.GetService<ConfigurationService>().AppSettings;
            _downloader = new ProjectDownloader(_auth, _http);
            LayerManager.Instance.OnProjectChanged -= OnProjectChanged;
            LayerManager.Instance.OnProjectChanged += OnProjectChanged;
            LayerManager.Instance.OnProjectUnloaded -= OnProjectUnloaded;
            LayerManager.Instance.OnProjectUnloaded += OnProjectUnloaded;
            await UniTask.CompletedTask;
        }

        public override async UniTask LoadDataAsync()
        {
            await LoadForTabAsync(ProjectLibraryTab.Mine);
        }

        private void OnProjectChanged(Project p)
        {
            if (p == null) return; // project unloaded
            SetActiveProject(p.Id);
        }

        private void OnProjectUnloaded()
        {
            ClearActiveProject();
        }

        private async UniTask<T> GetDataFromEndpointAsync<T>(string endpoint)
        {
            var tokenProvider = _auth.GetTokenStorageProvider();
            var accessToken = tokenProvider.GetAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                ULog.Warning("No access token found.");
                return await UniTask.FromResult(default(T));
            }
            var baseUrl = _appConfig.ServerConfig.ApiBaseUrl;
            var url = $"{baseUrl?.TrimEnd('/')}{endpoint}";
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

        public async UniTask LoadForTabAsync(ProjectLibraryTab tab)
        {
            CurrentTab = tab;

            ProjectLibraryItems mine = null;
            ProjectLibraryItems shared = null;

            try
            {
                mine = await GetDataFromEndpointAsync<ProjectLibraryItems>(_appConfig.ServerConfig.EndpointPath("ProjectList"));
                shared = await GetDataFromEndpointAsync<ProjectLibraryItems>(_appConfig.ServerConfig.EndpointPath("ProjectListShared"));
            }
            catch (Exception ex)
            {
                ULog.Error("Fetching Project Lists failed: " + ex.Message);
            }

            _mine.Clear();
            _shared.Clear();

            if (mine != null)
            {
                _mine.AddRange(mine);
            }

            if (shared != null)
            {
                _shared.AddRange(shared);
            }

            // Offline projects are always considered available and merged into both lists,
            // deduplicated by Id so they appear exactly once per tab.
            var offlineProjects = await _downloader.GetOfflineProjectsAsync(CancellationToken);
            if (offlineProjects != null)
            {
                foreach (var p in offlineProjects)
                {
                    var item = new ProjectLibraryItem
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    };

                    if (!_mine.Any(x => x.Id == item.Id))
                    {
                        _mine.Add(item);
                    }

                    if (!_shared.Any(x => x.Id == item.Id))
                    {
                        _shared.Add(item);
                    }
                }
            }

            _current = tab == ProjectLibraryTab.Mine ? _mine : _shared;
        }

        public string GetTilesBaseUrl()
        {
            return _appConfig.ServerConfig.TilesBaseUrl;
        }

        public void SetActiveProject(string id) 
        {
            if (ActiveProjectId == id) return;
            ActiveProjectId = id;
            ActiveProjectIdChanged?.Invoke(ActiveProjectId);
        }

        public void ClearActiveProject() 
        {
            if (ActiveProjectId == null) return;
            ActiveProjectId = null;
            ActiveProjectIdChanged?.Invoke(null);
        }
        
        public async UniTask<(Project project, string json)> GetProjectWithJsonAsync(string id)
        {
            try
            {
                var endpoint = _appConfig.ServerConfig.Endpoints.First(e => e.Name == "Project").Path.Replace("{id}", id);
                var projectJson = await GetDataFromEndpointAsync<string>(endpoint);
                if (string.IsNullOrWhiteSpace(projectJson))
                {
                    return (null, null);
                }

                var project = Project.FromJson(projectJson);
                return (project, projectJson);
            }
            catch (Exception ex)
            {
                ULog.Error("Fetching Project (with JSON) failed: " + ex.Message);
                return (null, null);
            }
        }

        public async UniTask<Project> GetProjectAsync(string id)
        {
            var (project, _) = await GetProjectWithJsonAsync(id);
            return project;
        }
    }
}