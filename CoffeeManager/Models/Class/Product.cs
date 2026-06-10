using System.Collections.Generic;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents a product sold in the coffee shop.
    /// Handles pricing, stock, categories and warehouse-based recipes. (≧◡≦) ZORRODEV2026
    /// </summary>
    public class Product
    {
        #region DATA

        /// <summary>
        /// Unique identifier for the product. (≧◡≦)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Display name of the product. (≧◡≦)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Unit price of the product. (≧◡≦)
        /// </summary>
        public decimal Price { get; set; }

        public string ImagePath { get; set; } = "";
        public string Description { get; set; } = "";

        /// <summary>
        /// Current stock available. Used for bakery items and desserts. (≧◡≦)
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// True if this product uses warehouse ingredients (coffee drinks).
        /// False if it only uses its own stock (bread, desserts). (≧◡≦)
        /// </summary>
        public bool UsesWarehouse { get; set; }

        /// <summary>
        /// Recipe used to prepare one unit of this product when UsesWarehouse is true. (≧◡≦)
        /// </summary>
        public List<RecipeItem> Recipe { get; set; } = new();

        #endregion


        #region CATEGORY

        /// <summary>
        /// List of categories assigned to this product. (ಠ_ಠ)
        /// </summary>
        public List<string> Categories { get; set; } = new();

        /// <summary>
        /// Adds a new category if it does not already exist. (≧◡≦)
        /// </summary>
        public void AddCategory(string category)
        {
            if (!string.IsNullOrWhiteSpace(category) && !Categories.Contains(category))
                Categories.Add(category);
        }

        /// <summary>
        /// Removes a category from the product. (≧◡≦)
        /// </summary>
        public void RemoveCategory(string category)
        {
            Categories.Remove(category);
        }

        /// <summary>
        /// Checks if the product belongs to the specified category. (≧◡≦)
        /// </summary>
        public bool HasCategory(string category)
        {
            return Categories.Contains(category);
        }

        #endregion


        #region STOCK

        /// <summary>
        /// Increases stock by a positive amount. (≧◡≦)
        /// </summary>
        public void IncreaseStock(int amount)
        {
            if (amount > 0)
                Stock += amount;
        }

        /// <summary>
        /// Decreases stock if enough units are available. (≧◡≦)
        /// </summary>
        public bool DecreaseStock(int amount)
        {
            if (amount <= 0 || amount > Stock)
                return false;

            Stock -= amount;
            return true;
        }

        /// <summary>
        /// Returns true if at least one unit is available. (≧◡≦)
        /// </summary>
        public bool IsAvailable()
        {
            return Stock > 0;
        }

        #endregion


        #region COST

        /// <summary>
        /// Calculates the estimated cost of one unit of this product
        /// based on its recipe and warehouse item costs. (≧◡≦)
        /// </summary>
        public decimal GetUnitCost(Warehouse warehouse)
        {
            if (!UsesWarehouse || Recipe.Count == 0)
                return 0m;

            decimal total = 0m;

            foreach (var ingredient in Recipe)
            {
                var item = warehouse.GetItemById(ingredient.IngredientId);
                if (item == null) continue;

                total += ingredient.Quantity * item.CostPerUnit;
            }

            return total;
        }

        /// <summary>
        /// Calculates the estimated profit per unit (Price - Cost). (≧◡≦)
        /// </summary>
        public decimal GetEstimatedProfit(Warehouse warehouse)
        {
            return Price - GetUnitCost(warehouse);
        }

        #endregion


        #region VALIDATION

        /// <summary>
        /// Checks if the product can be prepared based on warehouse availability.
        /// Returns warnings but does NOT consume anything. (≧◡≦)
        /// </summary>
        public List<string> CanPrepare(Warehouse warehouse, int quantity)
        {
            var warnings = new List<string>();

            if (!UsesWarehouse)
                return warnings;

            foreach (var ingredient in Recipe)
            {
                var item = warehouse.GetItemById(ingredient.IngredientId);

                if (item == null)
                {
                    if (ingredient.IsOptional)
                        warnings.Add($"Optional ingredient '{ingredient.IngredientName}' is not registered. (≧◡≦)");
                    else
                        warnings.Add($"Missing required ingredient '{ingredient.IngredientName}'. (╥﹏╥)");
                    continue;
                }

                var totalNeeded = ingredient.Quantity * quantity;

                if (item.Quantity < totalNeeded)
                {
                    if (ingredient.IsOptional)
                        warnings.Add($"Optional ingredient '{ingredient.IngredientName}' is low. Continuing. (≧◡≦)");
                    else
                        warnings.Add($"Not enough '{ingredient.IngredientName}' to prepare {quantity}. (╥﹏╥)");
                }
            }

            return warnings;
        }

        #endregion


        #region PREPARATION

        /// <summary>
        /// Simulates preparation without consuming warehouse items. (≧◡≦)
        /// </summary>
        public Dictionary<string, decimal> SimulatePreparation(int quantity)
        {
            var usage = new Dictionary<string, decimal>();

            if (!UsesWarehouse)
                return usage;

            foreach (var ingredient in Recipe)
                usage[ingredient.IngredientName] = ingredient.Quantity * quantity;

            return usage;
        }

        /// <summary>
        /// Prepares the product and consumes ingredients from the warehouse.
        /// Returns warnings for missing or low ingredients. (≧◡≦)
        /// </summary>
        public List<string> PrepareAndConsume(Warehouse warehouse, int quantity)
        {
            var warnings = new List<string>();

            if (!UsesWarehouse)
            {
                if (!DecreaseStock(quantity))
                    warnings.Add($"Not enough stock of '{Name}' to sell {quantity}. (╥﹏╥)");
                return warnings;
            }

            foreach (var ingredient in Recipe)
            {
                var item = warehouse.GetItemById(ingredient.IngredientId);
                if (item == null)
                {
                    if (ingredient.IsOptional)
                        warnings.Add($"Optional '{ingredient.IngredientName}' missing. Continuing. (≧◡≦)");
                    else
                        warnings.Add($"Missing required '{ingredient.IngredientName}'. (╥﹏╥)");
                    continue;
                }

                // 🔥 CONVERSIÓN DE UNIDADES
                decimal neededBase = UnitConverter.Convert(
                    ingredient.Unit,
                    item.Unit,
                    ingredient.Quantity * quantity
                );

                if (item.Quantity < neededBase && !ingredient.IsOptional)
                    warnings.Add($"'{ingredient.IngredientName}' will go negative. (╥﹏╥)");

                item.Consume(neededBase);
            }

            return warnings;
        }




        #endregion
    }
}
