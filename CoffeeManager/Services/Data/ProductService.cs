using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CoffeeManager.Models.Class;
using CoffeeManager.Services;

namespace CoffeeManager.Services.Data
{
    internal static class ProductService
    {
        private static readonly string FilePath = PathService.Products;

        public static List<Product> Load()
        {
            if (!File.Exists(FilePath))
                return new List<Product>();

            var json = File.ReadAllText(FilePath);

            var list = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return list ?? new List<Product>();
        }

        public static void Save(List<Product> products)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }
    }
}
