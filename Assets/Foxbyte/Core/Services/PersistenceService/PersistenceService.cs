using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Foxbyte.Core.Services.PersistenceService
{
    public class PersistenceService : IPersistenceService, IAppServiceAsync
    {
        private IPersistenceHandler _handler;
        private Queue<UniTask> _saveQueue = new Queue<UniTask>();
        private bool _isProcessing = false;

        public PersistenceService()
        {
        }

        public PersistenceService(IPersistenceHandler handler)
        {
            InitService(handler);
        }
        public async UniTask InitServiceAsync()
        {
            await UniTask.CompletedTask;
        }

        public async UniTask DisposeServiceAsync()
        {
            if (_handler != null)
            {
                _handler = null;
            }
            _saveQueue.Clear();
            _isProcessing = false;
            await UniTask.CompletedTask;
        }
        
        public void InitService(IPersistenceHandler handler)
        {
            _handler = handler;
        }

        public void QueueSaveOperation<T>(string key, T data)
        {
            _saveQueue.Enqueue(SaveDataInternalAsync(key, data));
            ProcessQueueAsync().Forget();
        }

        private async UniTaskVoid ProcessQueueAsync()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            while (_saveQueue.Count > 0)
            {
                var task = _saveQueue.Dequeue();
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    ULog.Error($"Failed to save data: {ex.Message}");
                }
            }

            _isProcessing = false;
        }

        private async UniTask SaveDataInternalAsync<T>(string key, T data)
        {
            if (_handler == null)
            {
                ULog.Error("Persistence handler not initialized.");
                return;
            }
            await _handler.SaveDataAsync(key, data);
        }

        // This method is kept for backward compatibility
        public UniTask SaveDataAsync<T>(string key, T data)
        {
            //await UniTask.DelayFrame(1);
            QueueSaveOperation(key, data);
            return UniTask.CompletedTask;
        }

        public async UniTask<T> LoadDataAsync<T>(string key) where T : new()
        {
            if (_handler == null)
            {
                ULog.Error("Persistence handler not initialized.");
                return new T();
            }
            return await _handler.LoadDataAsync<T>(key);
        }

        public UniTask DeleteDataAsync(string key)
        {
            if (_handler == null)
            {
                ULog.Error("Persistence handler not initialized.");
                return UniTask.CompletedTask;
            }
            return _handler.DeleteDataAsync(key);
        }
    }

    /// <summary>
    /// Exists to wrap strings in a class
    /// so that they can be saved as a generic type.
    /// Saving a string directly will cause a serialization error,
    /// complaining about a missing parameterless constructor.
    public class StringWrapper
    {
        public string Value { get; set; }
        // parameterless constructor
        public StringWrapper() { } 
        public StringWrapper(string value)
        {
            Value = value;
        }
    }
}