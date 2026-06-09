using System;
using System.IO;
using System.Text;

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
