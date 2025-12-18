using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Foxbyte.Core
{
    public interface IAppServiceBase { }
    
    public interface IAppService : IAppServiceBase
    {
        void InitService();
        void DisposeService();
    }
    
    public interface IAppServiceAsync : IAppServiceBase
    {
        UniTask InitServiceAsync();
        UniTask DisposeServiceAsync();
    }
    
    /// <summary>
    /// Base class for a service locator that manages application services.
    /// Keep derived class as top level GameObject in your startup scene.
    /// Calls to an inheriting class can be made without calling Instance.
    /// </summary>
    public abstract class ServiceLocatorBase<T> : MonoBehaviour where T : ServiceLocatorBase<T>
    {
        private static T _instance;
        public static T Instance => _instance;
        
        private readonly Dictionary<Type, IAppServiceBase> _services = new();
        
        public bool IsInitialized { get; private set; }
        public static event Action OnServicesReady;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeAsync().Forget(ex =>
                ULog.Error(ex.ToString())
                );
        }

        public static async UniTask InitializeAsync()
        {
            await UniTask.Yield();
            await _instance.RegisterServicesAsync();
            _instance.IsInitialized = true;
            OnServicesReady?.Invoke();
        }

        public static TService GetService<TService>() where TService : class, IAppServiceBase
        {
            return _instance?.GetServiceBase<TService>();
        }

        public static async UniTask<bool> RegisterServiceAsync<TService>(TService service) where TService : class, IAppServiceBase
        {
            if (_instance == null) return false;
            return await _instance.RegisterServiceBaseAsync(service);
        }

        public static bool UnregisterService<TService>() where TService : class, IAppServiceBase
        {
            return _instance?.UnregisterServiceBase<TService>() ?? false;
        }

        public static bool IsReady => _instance?.IsInitialized ?? false;

        /// <summary>
        /// Safe way to register for services ready event that handles all timing scenarios
        /// </summary>
        public static void RegisterForServicesReady(Action callback)
        {
            if (_instance?.IsInitialized == true)
            {
                // Services already ready, invoke immediately
                callback?.Invoke();
            }
            else
            {
                // Services not ready yet, register for event
                OnServicesReady += callback;
            }
        }

        /// <summary>
        /// Unregister from services ready event
        /// </summary>
        public static void UnregisterFromServicesReady(Action callback)
        {
            OnServicesReady -= callback;
        }
        
        /// <summary>
        /// Register and init your services here, in the order you need.
        /// </summary>
        protected abstract UniTask RegisterServicesAsync();
        
        /// <summary>
        /// Register any service (sync or async) and initialize it immediately.
        /// Make any InitServiceAsync() that has no real I/O return UniTask.CompletedTask.
        /// </summary>
        public async UniTask<bool> RegisterServiceBaseAsync<TService>(TService service) where TService : class, IAppServiceBase
        {
            var type = typeof(TService);
            if (!_services.TryAdd(type, service))
            {
                ULog.Info($"Service of type {type.Name} already registered.");
                return false;
            }
            //ULog.Info($"Service of type {type.Name} registered.");

            switch (service)
            {
                case IAppService syncSvc:
                    syncSvc.InitService();
                    break;
                case IAppServiceAsync asyncSvc:
                    await asyncSvc.InitServiceAsync();
                    break;
            }
            return true;
        }
        
        public TService GetServiceBase<TService>() where TService : class, IAppServiceBase
        {
            if (_services.TryGetValue(typeof(TService), out var svc))
                return svc as TService;
            ULog.Error($"Service of type {nameof(TService)} not registered.");
            return null;
        }
        
        public bool UnregisterServiceBase<TService>() where TService : class, IAppServiceBase
        {
            switch (_services.TryGetValue(typeof(TService), out var svc))
            {
                case false:
                    ULog.Info($"Service of type {nameof(TService)} was not registered.");
                    return false;
                case true when svc is IAppServiceAsync asyncSvc:
                    asyncSvc.DisposeServiceAsync().Forget();
                    break;
                case true when svc is IAppService syncSvc:
                    syncSvc.DisposeService();
                    break;
            }
            if (_services.Remove(typeof(TService)))
            {
                //ULog.Info($"Service of type {nameof(TService)} unregistered.");
                return true;
            }
            ULog.Info($"Service of type {nameof(TService)} was not found.");
            return false;
        }

        public static async UniTask<bool> UnregisterAllServicesAsync()
        {
            if (_instance == null)
                return false;

            var serviceTypes = _instance._services.Keys.ToList();
            var disposalTasks = new List<UniTask>();

            foreach (var serviceType in serviceTypes)
            {
                var svc = _instance._services[serviceType];
                switch (svc)
                {
                    case IAppServiceAsync asyncSvc:
                        disposalTasks.Add(asyncSvc.DisposeServiceAsync());
                        break;
                    case IAppService syncSvc:
                        syncSvc.DisposeService();
                        break;
                    default:
                        ULog.Info($"Unknown service type on unregister: {serviceType.Name}");
                        break;
                }

                _instance._services.Remove(serviceType);
                ULog.Info($"Service of type {serviceType.Name} unregistered.");
            }

            if (disposalTasks.Count > 0)
                await UniTask.WhenAll(disposalTasks);

            _instance._services.Clear();
            ULog.Info("All services unregistered.");
            return true;
        }

        protected virtual void OnDestroy()
        {
            foreach (var svc in _services.Values)
            {
                try
                {
                    switch (svc)
                    {
                        case IAppService syncSvc:
                            syncSvc.DisposeService();
                            break;
                        case IAppServiceAsync asyncSvc:
                            asyncSvc.DisposeServiceAsync().Forget();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ULog.Error($"Exception during service disposal: {ex.Message}");
                }
            }
            _services.Clear();
            if (_instance == this)
            {
                _instance = null;
                //ULog.Info("ServiceLocator instance destroyed.");
            }
            else
            {
                ULog.Warning("ServiceLocator instance mismatch on destroy.");
            }
            IsInitialized = false;
            ULog.Info("All services disposed.");
        }
    }
}