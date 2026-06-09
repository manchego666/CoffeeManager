// ===============================================================
//  ZORRODEV 2026 — Inventory Engine
//  Authors: Christopher (≧◡≦), Daniel (ง'̀-'́)ง, Brayan (✧ω✧), Jesús (◕‿◕✿)
//  Description: Stock logic, conversions and alerts. (≧◡≦)
// ===============================================================

using System;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a warehouse item with professional packaging logic. (≧◡≦) ZORRODEV2026
    /// </summary>
    internal class InventoryItem
    {
        #region DATA

        public int Id { get; set; }
        public string Name { get; set; } = "";

        /// <summary>
        /// Total quantity in base units (pieces, ml, grams). (≧◡≦)
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Base unit (pcs, ml, g, kg). (≧◡≦)
        /// </summary>
        public string Unit { get; set; } = "pcs";

        /// <summary>
        /// Cost per base unit. (≧◡≦)
        /// </summary>
        public decimal CostPerUnit { get; set; }

        /// <summary>
        /// How many base units are inside one package. (≧◡≦)
        /// Example: 50 servilletas por paquete.
        /// </summary>
        public int UnitsPerPackage { get; set; } = 1;

        /// <summary>
        /// How many packages are inside one box. (≧◡≦)
        /// Example: 12 paquetes por caja.
        /// </summary>
        public int PackagesPerBox { get; set; } = 1;

        public DateTime LastUpdated { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Converts boxes/packages/pieces into base units. (≧◡≦)
        /// </summary>
        public decimal ConvertToBaseUnits(int boxes, int packages, decimal pieces)
        {
            decimal total =
                (boxes * PackagesPerBox * UnitsPerPackage) +
                (packages * UnitsPerPackage) +
                pieces;

            return total;
        }

        /// <summary>
        /// Adds stock using boxes, packages or pieces. (≧◡≦)
        /// </summary>
        public void AddStock(int boxes, int packages, decimal pieces)
        {
            Quantity += ConvertToBaseUnits(boxes, packages, pieces);
            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Consumes base units (pieces/ml/g). Prevents negative stock. (≧◡≦)
        /// </summary>
        public void Consume(decimal baseUnits)
        {
            Quantity -= baseUnits;
            if (Quantity < 0)
                Quantity = 0;

            LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Returns true if the item is out of stock. (╥﹏╥)
        /// </summary>
        public bool IsOutOfStock() => Quantity <= 0;

        /// <summary>
        /// Returns true if the item is low on stock. (✧ω✧)
        /// </summary>
        public bool IsLowStock(decimal min = 5) =>
            Quantity > 0 && Quantity <= min;

        #endregion
    }
}
