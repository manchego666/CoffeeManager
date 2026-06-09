using System.IO;

namespace CoffeeManager.Services
{
    internal static class PathService
    {
        public static string BasePath => "Data";

        public static string Products => Path.Combine(BasePath, "products.json");
        public static string Sales => Path.Combine(BasePath, "sales.json");
        public static string DailyReport => Path.Combine(BasePath, "daily_report.txt");
    }
}

