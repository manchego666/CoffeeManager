namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a single ingredient used to prepare a product. (≧◡≦) ZORRODEV2026
    /// Links directly to Warehouse items for accuracy. (≧◡≦)
    /// </summary>
    internal class RecipeItem
    {
        #region DATA

        /// <summary>
        /// ID of the ingredient in the warehouse. (≧◡≦)
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Display name of the ingredient (for UI only). (≧◡≦)
        /// </summary>
        public string IngredientName { get; set; } = string.Empty;

        /// <summary>
        /// Quantity required for one unit of the product. (≧◡≦)
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Unit of measurement (g, ml, pcs, etc.). (≧◡≦)
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// If true, lack of this ingredient will NOT block the sale (napkin, straw). (≧◡≦)
        /// </summary>
        public bool IsOptional { get; set; }

        #endregion
    }
}
