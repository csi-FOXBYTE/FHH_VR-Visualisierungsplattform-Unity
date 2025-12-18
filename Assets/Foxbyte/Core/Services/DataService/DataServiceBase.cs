using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Foxbyte.Core.Services.DataService.Storage;

namespace Foxbyte.Core.Services.DataService
{
    public abstract class DataServiceBase : IDataService, IAppService
    {
        protected readonly Dictionary<string, object> DataStore = new Dictionary<string, object>();
        protected readonly DataPersistenceConfig PersistenceConfig;
        protected readonly DataServiceConfig ServiceConfig;
        protected PersistenceService.PersistenceService PersistenceService { get; private set; }

        protected DataServiceBase(DataServiceConfig config)
        {
            ServiceConfig = config ?? throw new ArgumentNullException(nameof(config));
            PersistenceConfig = new DataPersistenceConfig();
        }

        public virtual void InitService()
        {
            if (PersistenceService == null)
            {
                //PersistenceService = ServiceLocator.Get<PersistenceService>();
                if (PersistenceService == null)
                {
                    // Create and register PersistenceService with default strategy
                    //PersistenceService = new PersistenceService();
                    //ServiceLocator.RegisterService<PersistenceService>(PersistenceService);
                }
            }
        }

        public void DisposeService()
        {
            throw new NotImplementedException();
        }

        public virtual void AddOrUpdate<T>(T data) where T : class
        {
            if (data == null)
            {
                ULog.Error("[DataServiceBase] Data cannot be null.");
                return;
            }

            DataStore[typeof(T).FullName] = data;
        }

        public virtual void AddOrUpdate<T>(string key, T data) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                ULog.Error("[DataServiceBase] Key cannot be null or empty.");
                return;
            }

            if (data == null)
            {
                ULog.Error("[DataServiceBase] Data cannot be null.");
                return;
            }

            DataStore[key] = data;
        }

        public virtual T Get<T>() where T : class
        {
            return Get<T>(typeof(T).FullName);
        }

        public virtual T Get<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                ULog.Error("[DataServiceBase] Key cannot be null or empty.");
                return null;
            }

            if (DataStore.TryGetValue(key, out var data))
            {
                if (data is T typedData)
                {
                    return typedData;
                }
                ULog.Error($"[DataServiceBase] Type mismatch for key '{key}'. Expected '{typeof(T).Name}' but found '{data.GetType().Name}'.");
            }
            else
            {
                ULog.Error($"[DataServiceBase] No data found for key '{key}'.");
            }
            return null;
        }

        public virtual void Remove<T>() where T : class
        {
            Remove(typeof(T).FullName);
        }

        public virtual void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                ULog.Error("[DataServiceBase] Key cannot be null or empty.");
                return;
            }

            if (DataStore.Remove(key))
            {
                ULog.Info($"[DataServiceBase] Data removed for key '{key}'.");
            }
        }

        public virtual void Clear()
        {
            DataStore.Clear();
            ULog.Info("[DataServiceBase] Main datastore cleared.");
        }

        public void SavePersistableData()
        {
            throw new NotImplementedException();
        }

        public void LoadPersistedData()
        {
            throw new NotImplementedException();
        }

        public virtual void ConfigurePersistence<T>(bool shouldPersist, bool requiresEncryption = false, 
            bool useSecureStorage = false, IStorageStrategy overridingStorageStrategy = null) where T : class
        {
            PersistenceConfig.Configure<T>(shouldPersist, requiresEncryption, useSecureStorage, overridingStorageStrategy);
        }

        public virtual void ConfigurePersistence(string key, bool shouldPersist, bool requiresEncryption = false, 
            bool useSecureStorage = false, IStorageStrategy overridingStorageStrategy = null)
        {
            PersistenceConfig.Configure(key, shouldPersist, requiresEncryption, useSecureStorage, overridingStorageStrategy);
        }

        protected IStorageStrategy GetEffectiveStrategy(PersistenceSettings settings)
        {
            if (settings.OverridingStorageStrategy != null)
                return settings.OverridingStorageStrategy;

            if (settings.UseSecureStorage && ServiceConfig.SecureStrategy != null)
                return ServiceConfig.SecureStrategy;

            if (settings.RequiresEncryption && ServiceConfig.EncryptedStrategy != null)
                return ServiceConfig.EncryptedStrategy;

            return ServiceConfig.DefaultStrategy;
        }

        public virtual async UniTask SavePersistableDataAsync()
        {
            //if (PersistenceService == null)
            //{
            //    ULog.Error("[DataServiceBase] Cannot save data: PersistenceService is not initialized.");
            //    return;
            //}

            //foreach (var kvp in DataStore)
            //{
            //    var settings = PersistenceConfig.GetSettings(kvp.Key);
            //    if (!settings.ShouldPersist) continue;

            //    try
            //    {
            //        var strategy = GetEffectiveStrategy(settings);
            //        await PersistenceService.SaveDataAsync(kvp.Key, kvp.Value, strategy);
            //    }
            //    catch (Exception ex)
            //    {
            //        ULog.Error($"[DataServiceBase] Failed to save data for key '{kvp.Key}': {ex.Message}");
            //    }
            //}
            await UniTask.CompletedTask;
        }

        public virtual async UniTask LoadPersistedDataAsync()
        {
            //if (PersistenceService == null)
            //{
            //    ULog.Error("[DataServiceBase] Cannot load data: PersistenceService is not initialized.");
            //    return;
            //}

            //var persistedKeys = PersistenceService.GetAllKeys();
            //foreach (var key in persistedKeys)
            //{
            //    try
            //    {
            //        var settings = PersistenceConfig.GetSettings(key);
            //        if (!settings.ShouldPersist) continue;

            //        var strategy = GetEffectiveStrategy(settings);
            //        var data = await PersistenceService.LoadDataAsync<object>(key, strategy);

            //        if (data != null)
            //        {
            //            DataStore[key] = data;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        ULog.Error($"[DataServiceBase] Failed to load data for key '{key}': {ex.Message}");
            //    }
            //}
            await UniTask.CompletedTask;
        }
    }
}
