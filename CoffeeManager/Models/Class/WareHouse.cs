using CoffeeManager.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents the internal warehouse of the coffee shop.
    /// Stores consumables, tools and supplies. (✧ω✧) ZORRODEV2026
    /// </summary>
    public class Warehouse
    {
        #region DATA

        /// <summary>
        /// List of all inventory items in the warehouse. (≧◡≦)
        /// </summary>
        public List<InventoryItem> Items { get; set; } = new();

        #endregion

        #region METHODS

        /// <summary>
        /// Adds a new item or increases quantity if it already exists. (ง'̀-'́)ง
        /// If the item exists, cost per unit can be updated optionally. (≧◡≦)
        /// </summary>
        public InventoryItem AddOrUpdateItem(string name, decimal quantity, string unit, decimal? costPerUnit = null)
        {
            if (!Enum.TryParse<UnitType>(unit, true, out var parsedUnit))
            {
                parsedUnit = UnitType.Piezas;
            }

            var item = Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new InventoryItem
                {
                    Id = Items.Count + 1,
                    Name = name,
                    Quantity = quantity,
                    Unit = parsedUnit,   
                    CostPerUnit = costPerUnit ?? 0m,
                    LastUpdated = DateTime.Now
                };
                Items.Add(item);
            }
            else
            {
                item.Quantity += quantity;
                if (costPerUnit.HasValue)
                    item.CostPerUnit = costPerUnit.Value;

                item.LastUpdated = DateTime.Now;
            }

            return item;
        }


        /// <summary>
        /// Consumes quantity from an item by name. Can go negative (debt). (ಠ‿ಠ)
        /// </summary>
        public void Consume(string name, decimal quantity)
        {
            var item = Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item == null) return;

            item.Quantity -= quantity;
            item.LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Consumes quantity from an item by Id. Can go negative (debt). (ಠ‿ಠ)
        /// </summary>
        public void ConsumeById(int id, decimal quantity)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item == null) return;

            item.Quantity -= quantity;
            item.LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Returns an item by name. (◕‿◕✿)
        /// </summary>
        public InventoryItem? GetItem(string name)
        {
            return Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns an item by Id. (◕‿◕✿)
        /// </summary>
        public InventoryItem? GetItemById(int id)
        {
            return Items.FirstOrDefault(x => x.Id == id);
        }

        #endregion
    }
}
