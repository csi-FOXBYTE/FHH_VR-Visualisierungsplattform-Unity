using Cysharp.Threading.Tasks;

namespace Foxbyte.Core
{
    public interface IPersistenceService
    {
        void InitService(IPersistenceHandler handler);
        UniTask SaveDataAsync<T>(string key, T data);
        UniTask<T> LoadDataAsync<T>(string key) where T : new();
        UniTask DeleteDataAsync(string key);
    }

}