using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foxbyte.Core.Services.DataService.Storage
{
    /// <summary>
    /// Platform-specific secure storage strategy.
    /// Uses Keychain on iOS, encrypted preferences on Android, and DPAPI on Windows.
    /// </summary>
    public class SecureStorageStrategy : IStorageStrategy
    {
        public string Name => "SecureStorage";
        public bool SupportsEncryption => true;
        public bool SupportsSecureStorage => true;
        public async UniTask SaveDataAsync<T>(string key, T data)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask<T> LoadDataAsync<T>(string key) where T : class
        {
            await UniTask.CompletedTask;
            return null;
        }

        public async UniTask DeleteDataAsync(string key)
        {
            await UniTask.CompletedTask;
        }


        // make async
        public void Save(string key, object data)
        {
#if UNITY_IOS
            // Use Keychain
            SaveToKeychain(key, data);
#elif UNITY_ANDROID
            // Use EncryptedSharedPreferences
            SaveToEncryptedPrefs(key, data);
#elif UNITY_STANDALONE_WIN
            // Use Windows DPAPI
            //SaveToWindowsSecureStorage(key, data);
#else
            ULog.Warning($"[SecureStorageStrategy] Platform {Application.platform} doesn't support secure storage. Data will not be saved.");
#endif
        }

        public T Load<T>(string key) where T : class
        {
#if UNITY_IOS
            return LoadFromKeychain<T>(key);
#elif UNITY_ANDROID
            return LoadFromEncryptedPrefs<T>(key);
#elif UNITY_STANDALONE_WIN
            //return LoadFromWindowsSecureStorage<T>(key);
            return null;
#else
            ULog.Warning($"[SecureStorageStrategy] Platform {Application.platform} doesn't support secure storage.");
            return null;
#endif
        }

        public bool HasKey(string key)
        {
#if UNITY_IOS
            return HasKeyInKeychain(key);
#elif UNITY_ANDROID
            return HasKeyInEncryptedPrefs(key);
#elif UNITY_STANDALONE_WIN
            //return HasKeyInWindowsSecureStorage(key);
            return false;
#else
            return false;
#endif
        }

        public void Delete(string key)
        {
#if UNITY_IOS
            DeleteFromKeychain(key);
#elif UNITY_ANDROID
            DeleteFromEncryptedPrefs(key);
#elif UNITY_STANDALONE_WIN
            //DeleteFromWindowsSecureStorage(key);
#endif
        }
    }
}