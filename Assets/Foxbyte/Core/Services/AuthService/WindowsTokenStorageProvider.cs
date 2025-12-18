using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foxbyte.Core.Services.AuthService
{
    public class WindowsTokenStorageProvider : BaseTokenStorageProvider
    {
        protected override async UniTask SaveValueAsync(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
            await UniTask.CompletedTask;
        }

        protected override async UniTask<string> GetValueAsync(string key)
        {
            string value = PlayerPrefs.GetString(key, null);
            await UniTask.CompletedTask;
            return value;
        }

        protected override async UniTask DeleteValueAsync(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            await UniTask.CompletedTask;
        }
    }
}