using System;
using Cysharp.Threading.Tasks;

namespace Foxbyte.Core.Services.DataService.Storage
{
    public interface IStorageStrategy
    {
        string Name { get; }
        bool SupportsEncryption { get; }
        bool SupportsSecureStorage { get; }
        
        UniTask SaveDataAsync<T>(string key, T data);
        UniTask<T> LoadDataAsync<T>(string key) where T : class;
        UniTask DeleteDataAsync(string key);
        bool HasKey(string key);
    }
}
