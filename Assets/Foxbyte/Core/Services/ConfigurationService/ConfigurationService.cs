using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace Foxbyte.Core.Services.ConfigurationService
{
    public class ConfigurationService : IAppServiceAsync
    {
        private const string FileName = "appconfig.json";
        private AppConfig _appConfig;
        private const float FloatEpsilon = 0.0001f;
        
        private const string UserKeyPrefix = "FB_";

        public AppConfig AppSettings => _appConfig;

        public ConfigurationService()
        {
        }

        public async UniTask InitServiceAsync()
        {
            await LoadAppConfigAsync();
        }

        public async UniTask DisposeServiceAsync()
        {
            _appConfig = null;
            await UniTask.CompletedTask;
        }

        private async UniTask LoadAppConfigAsync()
        {
            string path = Path.Combine(Application.streamingAssetsPath, FileName);

            try
            {
                string json = string.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            var operation = www.SendWebRequest();

            // Simple timeout guard (prevents indefinite stall).
            const int timeoutMs = 5000;
            int startMs = Environment.TickCount;

            while (!operation.isDone)
            {
                if (Environment.TickCount - startMs > timeoutMs)
                {
                    Debug.LogError($"ConfigurationService: Timeout reading configuration file via UnityWebRequest: {path}");
                    return;
                }

                await UniTask.Yield();
            }

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError($"ConfigurationService: Error reading configuration file - {www.error}");
                return;
            }

            json = www.downloadHandler.text;
        }
#else
                json = await UniTask.RunOnThreadPool(() =>
                {
                    if (!File.Exists(path))
                        throw new FileNotFoundException($"Configuration file not found at path: {path}");

                    return File.ReadAllText(path);
                });
#endif

                _appConfig = JsonConvert.DeserializeObject<AppConfig>(json);

                if (_appConfig == null || _appConfig.ServerConfig == null)
                {
                    Debug.LogError("ConfigurationService: Failed to deserialize configuration. The file may be empty or have an invalid format.");
                }
                else
                {
                    Debug.Log("ConfigurationService: Configuration loaded successfully.");
                }
            }
            catch (FileNotFoundException fnf)
            {
                Debug.LogError($"ConfigurationService: {fnf.Message}");
            }
            catch (JsonException jsonEx)
            {
                Debug.LogError($"ConfigurationService: JSON parsing error - {jsonEx.Message}");
            }
            catch (IOException ioEx)
            {
                Debug.LogError($"ConfigurationService: IO error - {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"ConfigurationService: Unexpected error - {ex.Message}");
            }
        }

        // User Settings Management

        /// <summary>
        /// Load all PlayerPrefs-compatible properties into a new instance of the provided model type.
        /// </summary>
        public T LoadUserSettings<T>() where T : new()
        {
            var result = new T();
            foreach (var p in GetPrefsProps(typeof(T)))
            {
                var key = BuildKey(typeof(T), p.Name);
                if (!UnityEngine.PlayerPrefs.HasKey(key)) continue;
                if (TryReadPref(key, p.PropertyType, out var value))
                    p.SetValue(result, value);
            }
            return result;
        }

        /// <summary>
        /// Overwrite-save: writes ALL supported properties of T into PlayerPrefs
        /// (creates or updates keys). Uses float tolerance to avoid redundant writes.
        /// </summary>
        public void SaveUserSettingsOverwrite<T>(T full) where T : new()
        {
            if (full == null) return;

            foreach (var p in GetPrefsProps(typeof(T)))
            {
                var key = BuildKey(typeof(T), p.Name);
                var incoming = p.GetValue(full);

                if (TryReadPref(key, p.PropertyType, out var current) &&
                    ValuesEqualWithTolerance(current, incoming, p.PropertyType))
                {
                    continue;
                }

                WritePref(key, incoming, p.PropertyType);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Patch-save: writes ONLY non-null properties found on TPatch into PlayerPrefs,
        /// mapping by property name to TSettings. Define nullable properties on TPatch to indicate.
        /// Allows to do partial updates safely without overwriting other fields.
        /// Usage: SaveUserSettingsPatch<UserSettings, UserSettingsPatch>(new UserSettingsPatch { Language = "English" });
        /// </summary>
        public void SaveUserSettingsPatch<TSettings, TPatch>(TPatch patch)
            where TSettings : new()
            where TPatch : class
        {
            if (patch == null) return;

            var patchProps = typeof(TPatch).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in patchProps)
            {
                if (!p.CanRead) continue;

                // Unwrap Nullable<T> for support check
                var sourceType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (!IsSupportedType(sourceType)) continue;

                var value = p.GetValue(patch);
                if (value == null) continue; // unset in the patch => skip

                // Find corresponding target property on TSettings (by name)
                var targetProp = typeof(TSettings).GetProperty(p.Name, BindingFlags.Instance | BindingFlags.Public);
                if (targetProp == null) continue;
                if (!IsSupportedType(targetProp.PropertyType)) continue;

                var key = BuildKey(typeof(TSettings), p.Name);

                if (TryReadPref(key, targetProp.PropertyType, out var current) &&
                    ValuesEqualWithTolerance(current, value, targetProp.PropertyType))
                {
                    continue;
                }

                WritePref(key, value, targetProp.PropertyType);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save a single PlayerPrefs-compatible property from the provided model type.
        /// Example: SaveUserSettingProperty<UserSettings, string>(u => u.Language, "German");
        /// </summary>
        /// <typeparam name="TSettings"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        public void SaveUserSettingProperty<TSettings, TProp>(
            Expression<Func<TSettings, TProp>> selector, TProp value)
        {
            if (selector?.Body is not MemberExpression { Member: PropertyInfo propInfo }) return;
            if (!IsSupportedType(propInfo.PropertyType)) return;

            var key = BuildKey(typeof(TSettings), propInfo.Name);

            if (TryReadPref(key, propInfo.PropertyType, out var current) &&
                ValuesEqualWithTolerance(current, value, propInfo.PropertyType))
                return;

            WritePref(key, value, propInfo.PropertyType);
            PlayerPrefs.Save();
        }
        
        private static string BuildKey(System.Type t, string propName)
        {
            return $"{UserKeyPrefix}{t.Name}_{propName}";
        }

        private static IEnumerable<PropertyInfo> GetPrefsProps(System.Type t)
        {
            return t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite && IsSupportedType(p.PropertyType));
        }

        private static bool IsSupportedType(System.Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type == typeof(string)
                || type == typeof(int)
                || type == typeof(float)
                || type == typeof(bool);
        }

        private static bool TryReadPref(string key, System.Type type, out object value)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string)) { value = PlayerPrefs.GetString(key, string.Empty); return true; }
            if (type == typeof(int)) { value = PlayerPrefs.GetInt(key); return true; }
            if (type == typeof(float)) { value = PlayerPrefs.GetFloat(key); return true; }
            if (type == typeof(bool)) { value = PlayerPrefs.GetInt(key) != 0; return true; }

            value = null; return false;
        }

        private static void WritePref(string key, object value, System.Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string)){ PlayerPrefs.SetString(key, value as string ?? string.Empty); return; }
            if (type == typeof(int)) { PlayerPrefs.SetInt(key, (int)value); return; }
            if (type == typeof(float)) { PlayerPrefs.SetFloat(key, (float)value); return; }
            if (type == typeof(bool)) { PlayerPrefs.SetInt(key, ((bool)value) ? 1 : 0); return; }
        }

        private static bool ValuesEqual(object a, object b, System.Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(float))
            {
                return EqualityComparer<float>.Default.Equals((float)(a ?? 0f), (float)(b ?? 0f));
            }

            return Equals(a, b);
        }

        private static bool ValuesEqualWithTolerance(object a, object b, System.Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(float))
            {
                var fa = a is float f1 ? f1 : 0f;
                var fb = b is float f2 ? f2 : 0f;
                return Math.Abs(fa - fb) <= FloatEpsilon;
            }

            return Equals(a, b);
        }
    }
}