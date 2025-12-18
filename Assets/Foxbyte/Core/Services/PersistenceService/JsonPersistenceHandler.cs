using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;

namespace Foxbyte.Core
{
    public class JsonPersistenceHandler : IPersistenceHandler
    {
        private readonly string _baseDirectory;
        private JsonSerializerSettings _settings;

        public JsonPersistenceHandler(string baseDirectory)
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
                //NullValueHandling = NullValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                //ContractResolver = new CamelCasePropertyNamesContractResolver()
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Converters = new List<JsonConverter>(), // optional custom converters if needed for specific types
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
            }
        }

        public async UniTask<T> LoadDataAsync<T>(string key) where T : new()
        {
            string filePath = _getFilePath(key);
            if (File.Exists(filePath))
            {
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
                }
            }
            else
            {
                ULog.Warning($"File not found: {filePath}");
            }

            return default;
        }

        public async UniTask DeleteDataAsync(string key)
        {
            string filePath = _getFilePath(key);
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    ULog.Info($"File deleted successfully: {filePath}");
                }
                catch (Exception ex)
                {
                    ULog.Error($"Failed to delete file at {filePath}: {ex.Message}");
                }
            }
            else
            {
                ULog.Warning($"File not found: {filePath}");
            }
            ULog.Info($"DeleteDataAsync completed for key: {key}");
            await UniTask.CompletedTask;
        }
    }
}