using System.Collections.Generic;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a product sold in the coffee shop.
    /// Contains pricing, stock control and category management. (≧◡≦) ZORRODEV2026
    /// </summary>
    internal class Product
    {
        #region DATA

        /// <summary>
        /// Unique identifier for the product (≧◡≦) ZORRODEV2026
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Display name of the product. (≧◡≦) ZORRODEV2026
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Unit price of the product. (≧◡≦) ZORRODEV2026
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Current stock available. Must be non-negative. (≧◡≦) ZORRODEV2026
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// List of categories assigned to this product.
        /// Categories are simple strings (no Category class required).(ಠ_ಠ)
        /// </summary>
        public List<string> Categories { get; set; } = new();

        #endregion

        #region METHODS

        /// <summary>
        /// Increases the stock by the specified amount. Amount must be positive. (≧◡≦) ZORRODEV2026
        /// </summary>
        /// <param name="amount">Amount to add to stock.</param> 
        public void IncreaseStock(int amount)
        {
            if (amount <= 0) return;
            Stock += amount;
        }

        /// <summary>
        /// Decreases the stock if enough units are available.
        /// Returns true if the operation was successful. (≧◡≦) ZORRODEV2026
        /// </summary>
        /// <param name="amount">Amount to subtract from stock.</param>
        public bool DecreaseStock(int amount)
        {
            if (amount <= 0 || amount > Stock)
                return false;

            Stock -= amount;
            return true;
        }

        /// <summary>
        /// Returns true if the product has at least one unit in stock. (≧◡≦) ZORRODEV2026
        /// </summary>
        public bool IsAvailable()
        {
            return Stock > 0;
        }

        /// <summary>
        /// Adds a new category to the product if it does not already exist and is not null or whitespace. (≧◡≦) ZORRODEV2026
        /// </summary>
        /// <param name="category">Category name to add.</param>
        public void AddCategory(string category)
        {
            if (!string.IsNullOrWhiteSpace(category) && !Categories.Contains(category))
                Categories.Add(category);
        }

        /// <summary>
        /// Removes a category from the product. (≧◡≦) ZORRODEV2026
        /// </summary>
        /// <param name="category">Category name to remove.</param>
        /// </summary>
        /// <param name="category">Category name to remove.</param>
        public void RemoveCategory(string category)
        {
            Categories.Remove(category);
        }

        /// <summary>
        /// Checks if the product belongs to the specified category. (≧◡≦) ZORRODEV2026
        /// </summary>
        /// <param name="category">Category name to check.</param>
        public bool HasCategory(string category)
        {
            return Categories.Contains(category);
        } 

        #endregion
    }
}
