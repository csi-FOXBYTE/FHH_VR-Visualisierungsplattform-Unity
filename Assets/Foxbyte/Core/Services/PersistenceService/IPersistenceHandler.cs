using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

namespace Foxbyte.Core
{
    public interface IPersistenceHandler
    {
        UniTask SaveDataAsync<T>(string key, T data);
        UniTask<T> LoadDataAsync<T>(string key) where T : new();
        UniTask DeleteDataAsync(string key);
    }
}