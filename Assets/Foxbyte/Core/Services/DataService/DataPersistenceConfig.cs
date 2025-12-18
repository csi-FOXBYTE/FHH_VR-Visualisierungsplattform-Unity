using System.Collections.Generic;
using Foxbyte.Core.Services.DataService.Storage;

namespace Foxbyte.Core.Services.DataService
{
    public class DataPersistenceConfig
    {
        private readonly Dictionary<string, PersistenceSettings> _settings = new Dictionary<string, PersistenceSettings>();
        
        public void Configure(string key, bool shouldPersist, bool requiresEncryption = false, 
            bool useSecureStorage = false, IStorageStrategy overridingStorageStrategy = null)
        {
            _settings[key] = new PersistenceSettings(shouldPersist, requiresEncryption, 
                useSecureStorage, overridingStorageStrategy);
        }

        public void Configure<T>(bool shouldPersist, bool requiresEncryption = false, 
            bool useSecureStorage = false, IStorageStrategy overridingStorageStrategy = null)
        {
            Configure(typeof(T).FullName, shouldPersist, requiresEncryption, 
                useSecureStorage, overridingStorageStrategy);
        }

        public PersistenceSettings GetSettings(string key)
        {
            return _settings.TryGetValue(key, out var settings) ? settings : PersistenceSettings.Default;
        }

        public void Clear()
        {
            _settings.Clear();
        }
    }

    public readonly struct PersistenceSettings
    {
        public static readonly PersistenceSettings Default = new PersistenceSettings(false, false, false, null);

        public bool ShouldPersist { get; }
        public bool RequiresEncryption { get; }
        public bool UseSecureStorage { get; }
        public IStorageStrategy OverridingStorageStrategy { get; }

        public PersistenceSettings(bool shouldPersist, bool requiresEncryption, bool useSecureStorage, 
            IStorageStrategy overridingStorageStrategy)
        {
            ShouldPersist = shouldPersist;
            RequiresEncryption = requiresEncryption;
            UseSecureStorage = useSecureStorage;
            OverridingStorageStrategy = overridingStorageStrategy;
        }
    }
}
