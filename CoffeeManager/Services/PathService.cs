using System;
using System.IO;

namespace CoffeeManager.Services
{
    internal static class PathService
    {
        private static readonly string BasePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CoffeeManager");

        static PathService()
        {
            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);
        }
        public static string Employees => Path.Combine(BasePath, "employees.json");
        public static string Products => Path.Combine(BasePath, "products.json");
        public static string Sales => Path.Combine(BasePath, "sales.json");
        public static string Warehouse => Path.Combine(BasePath, "warehouse.json");
        public static string Notifications => Path.Combine(BasePath, "notifications.json");
        public static string Login => Path.Combine(BasePath, "login.json");
        public static string DailyReport => Path.Combine(BasePath, "daily_report.txt");

    }
}
