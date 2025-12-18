using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foxbyte.Core.Services.DataService.Storage
{
    public class PlayerPrefsStorageStrategy : IStorageStrategy
    {
        public string Name => "PlayerPrefs";
        public bool SupportsEncryption => false;
        public bool SupportsSecureStorage => false;
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

        //public void Save(string key, object data)
        //{
        //    // Basic implementation - you might want to add more sophisticated serialization
        //    if (data is string strData)
        //    {
        //        PlayerPrefs.SetString(key, strData);
        //    }
        //    else if (data is int intData)
        //    {
        //        PlayerPrefs.SetInt(key, intData);
        //    }
        //    else if (data is float floatData)
        //    {
        //        PlayerPrefs.SetFloat(key, floatData);
        //    }
        //    else
        //    {
        //        var json = JsonUtility.ToJson(data);
        //        PlayerPrefs.SetString(key, json);
        //    }
        //    PlayerPrefs.Save();
        //}

        //public T Load<T>(string key) where T : class
        //{
        //    if (!PlayerPrefs.HasKey(key)) return null;
            
        //    var value = PlayerPrefs.GetString(key);
        //    return JsonUtility.FromJson<T>(value);
        //}
        //public void Delete(string key)
        //{
        //    PlayerPrefs.DeleteKey(key);
        //    PlayerPrefs.Save();
        //}

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

    }
}
