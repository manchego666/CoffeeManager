namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a single item inside a sale. (✿◠‿◠) ZORRODEV2026
    /// </summary>
    internal class SaleDetail
    {
        #region DATA (≧◡≦)

        /// <summary>
        /// Unique identifier for the sale detail. (◕‿◕✿)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product associated with this sale detail. (✧ω✧)
        /// </summary>
        public Product Product { get; set; } = new();

        /// <summary>
        /// Quantity of the product sold. (ಠ‿ಠ)
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Returns the ID of the product in this detail. (ง'̀-'́)ง ZORRODEV2026
        /// </summary>
        public int ProductId => Product.Id;

        #endregion

        #region METHODS (◕‿◕✿)

        /// <summary>
        /// Calculates the subtotal for this sale detail. (≧◡≦)
        /// </summary>
        public decimal CalculateSubtotal()
        {
            return Product.Price * Quantity;
        }

        #endregion
    }
}
