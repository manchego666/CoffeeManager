using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CoffeeManager.Services
{
    internal class JsonService
    {
        public void Save<T>(string path, List<T> data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }

        public List<T> Load<T>(string path)
        {
            if (!File.Exists(path))
                return new List<T>();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
    }
}
