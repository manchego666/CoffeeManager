using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a sale transaction in the coffee shop. (≧◡≦) ZORRODEV2026
    /// </summary>
    internal class Sale
    {
        #region DATA

        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Employee who performed the sale. (≧◡≦)
        /// </summary>
        public Employee Employee { get; set; } = new();
        public int EmployeeId => Employee.Id;

        /// <summary>
        /// List of sale details. (≧◡≦)
        /// </summary>
        public List<SaleDetail> Details { get; set; } = new();

        #endregion


        #region METHODS

        /// <summary>
        /// Adds a product to the sale. (≧◡≦)
        /// </summary>
        public void AddProduct(Product product, int quantity)
        {
            if (quantity <= 0) return;

            var detail = Details.FirstOrDefault(d => d.Product.Id == product.Id);

            if (detail == null)
            {
                Details.Add(new SaleDetail
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                detail.Quantity += quantity;
            }
        }

        /// <summary>
        /// Applies the sale to the store (warehouse + stock). (≧◡≦)
        /// </summary>
        public List<string> ApplyToStore(Store store)
        {
            var warnings = new List<string>();

            foreach (var detail in Details)
            {
                var product = detail.Product;

                if (product == null)
                {
                    warnings.Add("Product is null in SaleDetail. (╥﹏╥)");
                    continue;
                }

                warnings.AddRange(product.CanPrepare(store.Warehouse, detail.Quantity));
                warnings.AddRange(product.PrepareAndConsume(store.Warehouse, detail.Quantity));
            }

            return warnings;
        }

        /// <summary>
        /// Total price of the sale. (≧◡≦)
        /// </summary>
        public decimal Total => Details.Sum(d => d.CalculateSubtotal());

        #endregion
    }
}
