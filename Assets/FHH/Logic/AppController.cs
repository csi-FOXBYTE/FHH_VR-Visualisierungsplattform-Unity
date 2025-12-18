using Cysharp.Threading.Tasks;
using FHH.Logic.Cesium;
using FHH.Logic.Components.HmdPresenceMonitor;
using FHH.Logic.Models;
using FHH.UI;
using FHH.UI.Disclaimer;
using FHH.UI.MainMenu;
using FHH.UI.ToolBar;
using FHH.UI.User;
using Foxbyte.Core;
using Foxbyte.Core.Localization.Utilities;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace FHH.Logic
{
    public class AppController : MonoBehaviour
    {
        //[SerializeField] private string _startupSceneName = "Startup";
        [SerializeField] private string _introSceneName = "Intro";
        [SerializeField] private string[] _scenesToPreload;
        [Range(1f, 10f)][SerializeField] private float _introDuration = 3f;
        [SerializeField] private bool _skipIntroInEditor = false;
        [SerializeField] private string _testSceneInEditor = String.Empty;
        [SerializeField] private bool _testSceneInEditorWithPreloadScenes = true;
        [Header("UI")]
        [SerializeField] private PanelSettings _psOverlay;
        [SerializeField] private PanelSettings _psWorldSpace;
        [SerializeField] private PanelInputConfiguration _psInputConfig;
        private UIDocument _uiDocument;
        private IPermissionService _permissionService;
        private CesiumTilesLoadingProgressProvider _tilesLoadingProgressProvider;
        
        private List<AsyncOperation> _backgroundOps = new List<AsyncOperation>();
        private AsyncOperation _introOp;

        void Awake()
        {
            Application.targetFrameRate = 90;
            ServiceLocator.RegisterForServicesReady(OnServicesInitialized);
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            
        }

        //void OnApplicationFocus(bool hasFocus)
        //{
        //    if (hasFocus)
        //    {
        //        Cursor.lockState = CursorLockMode.Locked;
        //        Cursor.visible = true;
        //    }
        //}

        private void OnServicesInitialized()
        {
            HandleServicesInitializedAsync().Forget(ex =>
                ULog.Error("Exception during Service init:" + ex.ToString())
                );
            _permissionService = ServiceLocator.GetService<PermissionService>();
            _permissionService.UserBecameAnonymous += OnUserBecameAnonymous;
            _permissionService.UserChanged += OnUserChanged;

            var configService = ServiceLocator.GetService<ConfigurationService>();
            var userSettings = configService.LoadUserSettings<UserSettings>();
            SetPerformanceMode(userSettings.PerformanceMode);
            //SetLanguage(userSettings.Language);
        }

        private void SetPerformanceMode(bool enable)
        {
            var name = enable ? "Performance" : "Beauty";
            var names = QualitySettings.names;
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] == name)
                {
                    QualitySettings.SetQualityLevel(i, true);
                    
                    break;
                }
            }
        }

        private void SetLanguage(string language)
        {
            ServiceLocator.GetService<LocaleSwitcher>().SwitchLocale(language);
        }

        private async void OnUserBecameAnonymous(object sender, EventArgs e)
        {
            await UniTask.WaitForEndOfFrame();
            ResetAppForAnonymousUserAsync().Forget();
        }

        private void OnUserChanged(object sender, PermissionService.UserChangedEventArgs e)
        {
            OnAnyUserChanged(e);
        }

        private void OnAnyUserChanged(PermissionService.UserChangedEventArgs userChangedEventArgs)
        {
            Debug.Log($"User changed from {userChangedEventArgs.OldUser.DisplayName} to {userChangedEventArgs.NewUser.DisplayName}");
        }

        private async UniTask HandleServicesInitializedAsync()
        {
            await UniTask.DelayFrame(10);
            bool skipIntro = _skipIntroInEditor && Application.isEditor;

            // optional test scene
            if (Application.isEditor && !string.IsNullOrEmpty(_testSceneInEditor))
            {
                await LoadTestSceneAsync();
            }

            // intro
            if (!skipIntro)
            {
                _introOp = SceneManager.LoadSceneAsync(_introSceneName, LoadSceneMode.Additive);
                if (_introOp == null)
                    throw new InvalidOperationException($"Failed to start loading intro scene '{_introSceneName}'. Ensure it's in Build Settings.");
                await _introOp.ToUniTask();
            }
            
            // preload scenes
            Scene lastScene = default;
            string lastSceneName = _scenesToPreload.Length > 0 ? _scenesToPreload[^1] : string.Empty;

            foreach (var sceneName in _scenesToPreload)
            {
                var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                if (op == null)
                    throw new InvalidOperationException($"Failed to preload scene '{sceneName}'. Ensure it is added to the build settings.");
                op.allowSceneActivation = false;
                _backgroundOps.Add(op);
                //lastScene = SceneManager.GetSceneByName(sceneName);
                lastSceneName = sceneName;
            }

            // show intro for a specified duration if not skipped
            if (!skipIntro && !_skipIntroInEditor && _introDuration > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_introDuration));
            }

            // wait until all background operations are at least 90% done
            foreach (var op in _backgroundOps)
            {
                while (op.progress < 0.9f)
                {
                    await UniTask.Yield();
                }
            }
            
            // activate all background operations
            foreach (var op in _backgroundOps)
            {
                op.allowSceneActivation = true;
            }

            // wait until activation of all background operations is complete
            foreach (var op in _backgroundOps)
            {
                await op.ToUniTask();
            }

            lastScene = SceneManager.GetSceneByName(lastSceneName);

            if (lastScene.IsValid())
            {
                SceneManager.SetActiveScene(lastScene);
            }

            await SceneManager.UnloadSceneAsync(_introSceneName).ToUniTask();

            await UniTask.DelayFrame(2);

            // UI initialization
            var uiManager = ServiceLocator.GetService<UIManager>();
            _uiDocument = FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .FirstOrDefault(go => go.name == "UI")?.GetComponent<UIDocument>();
            await uiManager.InitializeRootAsync(_uiDocument, _psOverlay, _psWorldSpace, null, _psInputConfig);
            //uiManager.SetUIMode(UIMode.WorldSpace, null);
            
            // create UI
            await SetupUiAsync();

            try
            {
                var hmdService = ServiceLocator.GetService<HmdPresenceMonitorService>();
                if (hmdService != null)
                {
                    if (!hmdService.CheckIfXRDeviceIsPresent())
                    {
                        uiManager.SetUIMode(UIMode.Overlay, null);
                    }
                    else
                    {
                        uiManager.SetUIMode(UIMode.WorldSpace, null);
                    }
                }
            }
            catch (Exception ex)
            {
                uiManager.SetUIMode(UIMode.Overlay, null);
                Debug.LogWarning($"Failed to set UI mode based on HMD presence: {ex.Message}");
            }
            
            if (ServiceLocator.Instance.IsOffline)
            {
                Debug.LogWarning("Application is running in OFFLINE mode. Online features will be unavailable.");
            }

            await UniTask.Yield();
            SetPostProcessing();
            _tilesLoadingProgressProvider = FindFirstObjectByType<CesiumTilesLoadingProgressProvider>();
            if (_tilesLoadingProgressProvider == null)
            {
                Debug.LogError("CesiumTilesLoadingProgressProvider not found in the scene.");
            }
            else
            {
                _tilesLoadingProgressProvider.TilesLoaded += OnTilesLoaded;
            }

            await UniTask.Yield();
            SetLanguage(ServiceLocator.GetService<ConfigurationService>().LoadUserSettings<UserSettings>().Language);

            await UniTask.WaitForSeconds(10f);
            await CheckAppVersionAsync();

            await UniTask.CompletedTask;
        }

        private void OnTilesLoaded()
        {
            var userSettings = ServiceLocator.GetService<ConfigurationService>().LoadUserSettings<UserSettings>();
            float maxScreenError = userSettings.PerformanceMode ? 30f : 20f;
            CesiumTilesetUtilities.SetMaximumScreenSpaceErrorOnAllTilesets(maxScreenError);
            _tilesLoadingProgressProvider.TilesLoaded -= OnTilesLoaded;
        }

        private async UniTask LoadTestSceneAsync()
        {
            if (!_testSceneInEditorWithPreloadScenes)
                _scenesToPreload = Array.Empty<string>();
            await SceneManager.LoadSceneAsync(_testSceneInEditor, LoadSceneMode.Additive);
        }


        public async UniTask ResetAppForAnonymousUserAsync()
        {
            var lm = LayerManager.Instance;
            if (lm == null)
            {
                
                Debug.LogError("LayerManager.Instance is null. Cannot unload project or reset.");
                return;
            }
            await lm.UnloadProjectAsync();
            await lm.ResetToGeneralLayersOnlyAsync();

            var uiManager = ServiceLocator.GetService<UIManager>();
            await uiManager.HideAsync<MainMenuPresenter>();
        }
        
        private void OnDestroy()
        {
            if (_permissionService != null)
            {
                _permissionService.UserBecameAnonymous -= OnUserBecameAnonymous;
                _permissionService.UserChanged -= OnUserChanged;
            }
            ServiceLocator.UnregisterFromServicesReady(OnServicesInitialized);
        }

        private async UniTask SetupUiAsync()
        {
            await CreateMenuBarAsync();
            await CreateToolBarAsync();
            
            CreateDisclaimerAsync().Forget();
        }

        private async UniTask CreateMenuBarAsync()
        {
            var options = new WindowOptions
            {
                Region = UIProjectContext.UIRegion.Header,
                StyleSheet = Resources.Load<StyleSheet>("MenuBar") 
                // Permissions = new RequiredPermissions { Roles = { "Admin" } }, // optional
            };

            //var model = new MenuBarModel { Title = "Menu" };
            var model = new MenuBarModel();

            var uiManager = ServiceLocator.GetService<UIManager>();
            await uiManager.ShowWindowAsync<MenuBarPresenter, MenuBarView, MenuBarModel>(model, options);
            


            // Login / User button
            var rightSlot = _uiDocument.rootVisualElement.Q<VisualElement>("MenuBar_Right");
            await uiManager.ShowWindowAsync<UserPresenter, UserView, UserModel>(
                model: null,
                options: new WindowOptions
                {
                    TargetContainer = rightSlot,
                    CenterScreen = false,
                    StyleSheet = Resources.Load<StyleSheet>("User"),
                },
                rightSlot
                );

            await UniTask.CompletedTask;
        }

        private async UniTask CreateToolBarAsync()
        {
            var options = new WindowOptions
            {
                Region = UIProjectContext.UIRegion.Sidebar,
                StyleSheet = Resources.Load<StyleSheet>("ToolBar") 
                // Permissions = new RequiredPermissions { Roles = { "Admin" } }, // optional
            };

            var model = new ToolBarModel();

            var uiManager = ServiceLocator.GetService<UIManager>();
            await uiManager.ShowWindowAsync<ToolBarPresenter, ToolBarView, ToolBarModel>(model, options);
            await UniTask.CompletedTask;
        }

        private async UniTask CreateDisclaimerAsync()
        {
            await UniTask.Yield();
            var options = new WindowOptions()
            {
                Region = UIProjectContext.UIRegion.Content1,
                StyleSheet = Resources.Load<StyleSheet>("Disclaimer")
            };
            //var model = new DisclaimerModel();
            var uiManager = ServiceLocator.GetService<UIManager>();
            uiManager.ShowWindowAsync<DisclaimerPresenter, DisclaimerView, DisclaimerModel>(model: null, options).Forget();
        }

        private async UniTask CheckAppVersionAsync()
        {
            var versionChecker = new AppVersionChecker();
            var result = await versionChecker.CheckForUpdateAsync();

            if (!result.IsSuccess)
            {
                ULog.Warning("Failed to check for app updates.");
                return;
            }

            if (result.IsUpdateAvailable)
            {
                ULog.Info($"New version available. Current: {result.CurrentVersion}, Remote: {result.RemoteVersion}");
                ULog.Info($"Download URL: {result.DownloadUrl}");
                //ServiceLocator.GetService<UIManager>().ShowNotification("NewVersion", 15f,
                //    () => { Application.OpenURL(result.DownloadUrl); }, result.DownloadUrl);
                ServiceLocator.GetService<UIManager>().ShowNotification("NewVersion", 15f,
                    () => { Application.OpenURL(result.DownloadUrl); }, "Download");
            }
            else
            {
                ULog.Info("App is up to date.");
            }
        }

        private void SetPostProcessing()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("No main camera found.");
                return;
            }

            UniversalAdditionalCameraData cameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>();

            if (cameraData == null)
            {
                Debug.Log("No UniversalAdditionalCameraData found on camera.");
                return;
            }

            var userSettings = ServiceLocator.GetService<ConfigurationService>().LoadUserSettings<UserSettings>();

            cameraData.renderPostProcessing = !userSettings.PerformanceMode;
            mainCamera.farClipPlane = userSettings.PerformanceMode ? 2000 : 3000;
        }
    }
}