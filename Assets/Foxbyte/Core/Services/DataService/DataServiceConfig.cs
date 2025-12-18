using System;
using Foxbyte.Core.Services.DataService.Storage;

namespace Foxbyte.Core
{
    public class DataServiceConfig
    {
        public IStorageStrategy DefaultStrategy { get; }
        public IStorageStrategy SecureStrategy { get; }
        public IStorageStrategy EncryptedStrategy { get; }

        public DataServiceConfig(
            IStorageStrategy defaultStrategy,
            IStorageStrategy secureStrategy = null,
            IStorageStrategy encryptedStrategy = null)
        {
            DefaultStrategy = defaultStrategy ?? throw new ArgumentNullException(nameof(defaultStrategy));
            SecureStrategy = secureStrategy;
            EncryptedStrategy = encryptedStrategy;
        }
    }
}
