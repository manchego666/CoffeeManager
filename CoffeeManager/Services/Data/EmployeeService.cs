using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CoffeeManager.Models.Class;
using CoffeeManager.Services; 

namespace CoffeeManager.Services.Data
{
    internal static class EmployeeService
    {
        private static readonly string FilePath = PathService.Employees;
        public static List<Employee> Load()
        {
            if (!File.Exists(FilePath))
                return new List<Employee>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
        }

        public static void Save(List<Employee> employees)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(employees, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);
        }
    }
}
    