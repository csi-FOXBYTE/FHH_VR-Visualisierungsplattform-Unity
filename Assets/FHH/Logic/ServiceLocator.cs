using Cysharp.Threading.Tasks;
using FHH.Logic.Components.Collaboration;
using FHH.Logic.Components.HmdPresenceMonitor;
using FHH.Logic.Components.Networking;
using FHH.Logic.Models;
using FHH.UI;
using Foxbyte.Core;
using Foxbyte.Core.Localization.Utilities;
using Foxbyte.Core.Services.AuthService;
using Foxbyte.Core.Services.ConfigurationService;
using Foxbyte.Core.Services.Permission;
using Foxbyte.Core.Services.PersistenceService;
using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Networking;

namespace FHH.Logic
{
    /// <summary>
    /// Concrete implementation of the service locator for this project.
    /// Put on a top level gameObject in the startup scene.
    /// All needed services (sync or async) should be registered in the RegisterServicesAsync method.
    /// Await if needed. Return UniTask.CompletedTask if no async initialization is needed in the service.
    /// Automatically initializes services that implement IAppService(Async) interface.
    /// </summary>
    public class ServiceLocator : ServiceLocatorBase<ServiceLocator>
    {
        private bool _isOffline;
        public bool IsOffline => _isOffline;

        protected override async UniTask RegisterServicesAsync()
        {
            await UniTask.Yield();
            // give localization system time to initialize to avoid blocking "WaitForCompletion" issues
            await LocalizationSettings.InitializationOperation.Task;
            await UniTask.DelayFrame(10); // delay services registration

            // Example service registration and initialization
            // RegisterService<ISomeService>(new SomeServiceImplementation());
            // Retrieving a service is done with _someService = ServiceLocator.GetService<ISomeService>();

            // Standard services

            await RegisterServiceAsync(new ConfigurationService());
            var appConfig = GetService<ConfigurationService>().AppSettings;

            var healthCheckUrl = appConfig.ServerConfig.ApiBaseUrl + "/public/baseLayer/list";
            // quick short-circuit if device is obviously offline
            var reachability = Application.internetReachability;
            var obviouslyOffline = reachability == NetworkReachability.NotReachable;
            bool backendReachable = false;
            if (!obviouslyOffline)
            {
                backendReachable = await CheckBackendReachableAsync(healthCheckUrl, timeoutSeconds: 3);
            }

            _isOffline = obviouslyOffline || !backendReachable;
            if (_isOffline)
            {
                Debug.LogWarning("Backend is not reachable. Starting in OFFLINE mode.");
            }

            var httpClient = new HttpClientWithRetry();
            var windowsTokenStorageProvider = new WindowsTokenStorageProvider();

            await RegisterServiceAsync(new OAuth2AuthenticationService(
                windowsTokenStorageProvider,
                httpClient,
                new BrowserHandler(),
                appConfig
            ));

            //await RegisterServiceAsync(new DataTransferService(
            //    httpClient,
            //    windowsTokenStorageProvider,
            //    appConfig
            //));
            //await RegisterServiceAsync(new ServerEventService(
            //    httpClient,
            //    GetService<OAuth2AuthenticationService>(),
            //    appConfig,
            //    windowsTokenStorageProvider
            //));

            // register PermissionService with anonymous user. Re-Register with real user after login.
            await RegisterServiceAsync<PermissionService>(new PermissionService(User.Anonymous()));

            var jsonHandler = new JsonPersistenceHandler(UnityEngine.Application.persistentDataPath);
            await RegisterServiceAsync<PersistenceService>(new PersistenceService(jsonHandler));
            await RegisterServiceAsync<LocaleSwitcher>(new LocaleSwitcher());
            await UniTask.Yield();

            // Unity Gaming Services
            if (!_isOffline)
            {
                try
                {
                    await RegisterServiceAsync<UGSService>(new UGSService());
                }
                catch (TimeoutException ex)
                {
                    Debug.LogWarning($"UGS init timed out; starting OFFLINE. {ex}");
                    _isOffline = true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UGSService initialization failed: {ex}");
                    _isOffline = true; // can reach backend, but UGS failed
                }
            }

            await UniTask.Yield();

            await RegisterServiceAsync<UIManager>(new UIManager());

            // XR
            try
            {
                await RegisterServiceAsync<HmdPresenceMonitorService>(new HmdPresenceMonitorService());
            }
            catch (Exception ex)
            {
                Debug.LogError($"HmdPresenceMonitorService initialization failed: {ex.Message}");
            }

            // Collaboration
            if (!_isOffline)
            {
                try
                {
                    await RegisterServiceAsync<CollaborationService>(new CollaborationService());
                    await ServiceLocator.RegisterServiceAsync<CommandBusService>(new CommandBusService());
                }
                catch (Exception ex)
                {
                    Debug.LogError($"CollaborationService initialization failed: {ex.Message}");
                    _isOffline = true;
                }
            }

            await UniTask.CompletedTask;
        }

        private async UniTask<bool> CheckBackendReachableAsync(string testUrl, int timeoutSeconds = 3)
        {
            if (string.IsNullOrWhiteSpace(testUrl))
            {
                Debug.LogWarning("CheckBackendReachableAsync: testUrl is null or empty. Assuming OFFLINE.");
                return false;
            }

            if (!Uri.TryCreate(testUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                Debug.LogWarning($"CheckBackendReachableAsync: invalid url '{testUrl}'. Assuming OFFLINE.");
                return false;
            }

            int attempts = 2;
            float timeout = Mathf.Max(1, timeoutSeconds);

            for (int attempt = 1; attempt <= attempts; attempt++)
            {
                using (var request = UnityWebRequest.Get(uri))
                {
                    request.redirectLimit = 5;
                    request.SetRequestHeader("Cache-Control", "no-cache");

                    try
                    {
                        var op = request.SendWebRequest();

                        float start = Time.realtimeSinceStartup;
                        while (!op.isDone && (Time.realtimeSinceStartup - start) < timeout)
                            await UniTask.Yield(PlayerLoopTiming.Update);

                        if (!op.isDone)
                        {
                            request.Abort();
                            Debug.LogWarning(
                                $"Connectivity check to '{uri}' timed out after {timeoutSeconds}s (attempt {attempt}/{attempts}).");
                        }
                        else
                        {
                            // Any HTTP response code > 0 means we reached the server (even 401/403/404).
                            if (request.responseCode > 0)
                                return true;

                            Debug.LogWarning(
                                $"Connectivity check to '{uri}' failed: {request.result} {request.error} (attempt {attempt}/{attempts}).");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Absolute rule: never let init die here
                        Debug.LogWarning(
                            $"Connectivity check to '{uri}' threw: {ex.Message} (attempt {attempt}/{attempts}).");
                    }
                }

                if (attempt < attempts)
                    await UniTask.Delay(200);
            }

            return false;
        }
    }
}