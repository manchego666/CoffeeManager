using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CoffeeManager.Services
{
    /// <summary>
    /// Provides JSON serialization and deserialization utilities for lists and single objects.
    /// </summary>
    internal class JsonService
    {
        #region Save List<T>
        /// <summary>
        /// Saves a list of objects to a JSON file.
        /// </summary>
        public void Save<T>(string path, List<T> data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }
        #endregion

        #region Load List<T>
        /// <summary>
        /// Loads a list of objects from a JSON file.
        /// Returns an empty list if the file does not exist.
        /// </summary>
        public List<T> Load<T>(string path)
        {
            if (!File.Exists(path))
                return new List<T>();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
        #endregion

        #region SaveObject<T>
        /// <summary>
        /// Saves a single object to a JSON file.
        /// </summary>
        public void SaveObject<T>(string path, T data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }
        #endregion

        #region LoadObject<T>
        /// <summary>
        /// Loads a single object from a JSON file.
        /// Returns default(T) if the file does not exist.
        /// </summary>
        public T LoadObject<T>(string path)
        {
            if (!File.Exists(path))
                return default;

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json);
        }
        #endregion
    }
}
