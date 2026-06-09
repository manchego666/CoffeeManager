using CoffeeManager.Services;
using System;
using System.IO;
using System.Text;
using CoffeeManager.Services.Logic;
using System.Windows.Forms;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Handles generation and saving of daily TXT reports. (≧◡≦) ZORRODEV2026
    /// Uses ReportService for calculations and formatting. (≧◡≦)
    /// </summary>
    internal class DailyReportService
    {
        private readonly ReportService _reportService = new();

        #region GENERATION (≧◡≦)

        /// <summary>
        /// Generates the daily report text using ReportService. (≧◡≦)
        /// </summary>
        public string GenerateDailyReportText(Store store, DateTime date)
        {
            return _reportService.GenerateDailyReport(store, date);
        }

        #endregion

        public void GenerateDailyReport(Store store)
        {
            try
            {
                var report = new DailyReport
                {
                    Date = DateTime.Now,
                    TotalSales = store.Sales.Sum(s => s.Total),
                    TotalTransactions = store.Sales.Count,
                    Employees = store.Employees.Count,
                    Products = store.Products.Count
                };

                File.WriteAllText(PathService.DailyReport,
                    $"Corte Diario - {report.Date}\n" +
                    $"Ventas Totales: {report.TotalSales:C}\n" +
                    $"Transacciones: {report.TotalTransactions}\n" +
                    $"Empleados: {report.Employees}\n" +
                    $"Productos: {report.Products}\n");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generando corte diario:\n" + ex.Message);
            }
        }



        #region SAVE TO FILE (✧ω✧)

        /// <summary>
        /// Saves the daily report to a TXT file. (✧ω✧)
        /// Automatically creates the directory if needed. (≧◡≦)
        /// </summary>
        public void SaveDailyReportToFile(Store store, DateTime date, string filePath)
        {
            string report = GenerateDailyReportText(store, date);

            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(filePath, report, Encoding.UTF8);
        }

        #endregion
    }
}
