namespace Foxbyte.Core
{
    public interface IDataService
    {
        /// <summary>
        /// Adds or updates data using type as key
        /// </summary>
        void AddOrUpdate<T>(T data) where T : class;
        
        /// <summary>
        /// Adds or updates data using custom string key
        /// </summary>
        void AddOrUpdate<T>(string key, T data) where T : class;
        
        /// <summary>
        /// Gets data by type
        /// </summary>
        T Get<T>() where T : class;
        
        /// <summary>
        /// Gets data by string key
        /// </summary>
        T Get<T>(string key) where T : class;
        
        /// <summary>
        /// Removes data by type
        /// </summary>
        void Remove<T>() where T : class;
        
        /// <summary>
        /// Removes data by string key
        /// </summary>
        void Remove(string key);
        
        /// <summary>
        /// Clears all data from the store
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Saves all persistable data that has ShouldPersist=true
        /// </summary>
        void SavePersistableData();
        
        /// <summary>
        /// Loads all previously persisted data
        /// </summary>
        void LoadPersistedData();
    }
}