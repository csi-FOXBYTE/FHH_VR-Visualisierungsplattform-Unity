using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace Foxbyte.Core.Services.DataService.Storage
{
    /// <summary>
    /// Json storage strategy that saves data as JSON files in Application.persistentDataPath
    /// </summary>
    public class JsonStorageStrategy : IStorageStrategy
    {
        public string Name => "Json";
        public bool SupportsEncryption => false;
        public bool SupportsSecureStorage => false;

        private readonly string _baseDirectory;
        private JsonSerializerSettings _settings;

        public JsonStorageStrategy(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            _initializeSettings();
        }

        private void _initializeSettings()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>(),
                Error = _handleSerializationError
            };
            _settings.Converters.Add(new Vector3Converter());
            _settings.Converters.Add(new ColorConverter());
        }

        private void _handleSerializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            ULog.Error($"JSON Error: {e.ErrorContext.Error.Message}");
            e.ErrorContext.Handled = true;
        }

        private string _getFilePath(string key)
        {
            string fileName = $"{key}.json";
            string filePath = Path.Combine(_baseDirectory, fileName);
            if (!Directory.Exists(_baseDirectory))
            {
                Directory.CreateDirectory(_baseDirectory);
            }
            return filePath;
        }

        public async UniTask SaveDataAsync<T>(string key, T data)
        {
            string filePath = _getFilePath(key);
            string jsonData = JsonConvert.SerializeObject(data, _settings);

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                {
                    await writer.WriteAsync(jsonData);
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"Failed to save data to {filePath}: {ex.Message}");
                throw;
            }
        }

        public async UniTask<T> LoadDataAsync<T>(string key) where T : class
        {
            string filePath = _getFilePath(key);
            if (!File.Exists(filePath))
            {
                ULog.Warning($"File not found: {filePath}");
                return null;
            }

            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string jsonData = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<T>(jsonData, _settings);
                }
            }
            catch (Exception ex)
            {
                ULog.Error($"Failed to load data from {filePath}: {ex.Message}");
                throw;
            }
        }

        public async UniTask DeleteDataAsync(string key)
        {
            string filePath = _getFilePath(key);
            if (!File.Exists(filePath))
            {
                ULog.Warning($"File not found: {filePath}");
                return;
            }

            try
            {
                File.Delete(filePath);
                await UniTask.CompletedTask;
                ULog.Info($"File deleted successfully: {filePath}");
            }
            catch (Exception ex)
            {
                ULog.Error($"Failed to delete file at {filePath}: {ex.Message}");
                throw;
            }
        }

        public bool HasKey(string key)
        {
            return File.Exists(_getFilePath(key));
        }
    }
}
