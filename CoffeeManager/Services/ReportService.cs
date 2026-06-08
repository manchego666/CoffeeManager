using System;
using System.IO;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Services
{
    internal class ReportService
    {
        public void GenerateDailyReport(string path, Store store)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("DAILY REPORT - COFFEE MANAGER");
                writer.WriteLine($"Date: {DateTime.Now}");
                writer.WriteLine();

                decimal total = 0;

                foreach (var sale in store.Sales)
                {
                    writer.WriteLine($"Sale ID: {sale.Id}");
                    writer.WriteLine($"Employee: {sale.Employee.Name}");
                    writer.WriteLine($"Date: {sale.Date}");
                    writer.WriteLine("Products:");

                    foreach (var detail in sale.Details)
                    {
                        writer.WriteLine($" - {detail.Product.Name} x{detail.Quantity} = ${detail.Subtotal}");
                    }

                    writer.WriteLine($"Sale Total: ${sale.Total}");
                    writer.WriteLine("----------------------------------");

                    total += sale.Total;
                }

                writer.WriteLine();
                writer.WriteLine($"TOTAL OF THE DAY: ${total}");
            }
        }
    }
}
