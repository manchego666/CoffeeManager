using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CoffeeManager.Services
{
    public class JsonService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public void Save<T>(string path, List<T> data)
        {
            try
            {
                string json = JsonSerializer.Serialize(data, _options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving list: {ex.Message}");
            }
        }

        public List<T> Load<T>(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return new List<T>();

                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        public void SaveObject<T>(string path, T obj)
        {
            try
            {
                string json = JsonSerializer.Serialize(obj, _options);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving object: {ex.Message}");
            }
        }

        public T? LoadObject<T>(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return default;

                string json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<T>(json, _options);
            }
            catch
            {
                return default;
            }
        }
    }
}
