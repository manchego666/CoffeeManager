using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a sale made in the coffee shop. (≧◡≦) ZORRODEV2026
    /// </summary>
    internal class Sale
    {
        #region DATA (✿◠‿◠)

        /// <summary>
        /// Unique identifier for the sale. (◕‿◕✿)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the employee who made the sale. (✧ω✧)
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>
        /// Date and time of the sale. (≧◡≦)
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// List of sale details (products + quantities). (ಠ‿ಠ)
        /// </summary>
        public List<SaleDetail> Details { get; set; } = new();

        /// <summary>
        /// Total amount of the sale. (ง'̀-'́)ง ZORRODEV2026
        /// </summary>
        public decimal Total => CalculateTotal();

        #endregion

        #region METHODS (ง'̀-'́)ง

        /// <summary>
        /// Calculates the total amount of the sale. (◕‿◕✿)
        /// </summary>
        public decimal CalculateTotal()
        {
            return Details.Sum(d => d.CalculateSubtotal());
        }

        #endregion
    }
}
