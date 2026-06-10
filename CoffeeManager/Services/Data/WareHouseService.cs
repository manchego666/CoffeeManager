using System;
using System.IO;
using System.Text.Json;
using CoffeeManager.Models.Class;
using CoffeeManager.Services;

namespace CoffeeManager.Services.Data
{
    internal static class WarehouseService
    {
        private static readonly string FilePath = PathService.Warehouse;

        public static Warehouse Load()
        {
            if (!File.Exists(FilePath))
                return new Warehouse();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<Warehouse>(json) ?? new Warehouse();
        }

        public static void Save(Warehouse warehouse)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(warehouse, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }
    }
}
    