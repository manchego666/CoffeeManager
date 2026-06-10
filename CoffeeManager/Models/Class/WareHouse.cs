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

        #region ADD / UPDATE 

        /// <summary>
        /// Adds a new item or increases quantity if it already exists. (ง'̀-'́)ง
        /// If the item exists, cost per unit can be updated optionally. (≧◡≦)
        /// </summary>
        public InventoryItem AddOrUpdateItem(string name, decimal quantity, string unit, decimal? costPerUnit = null)
        {
            if (!Enum.TryParse<UnitType>(unit, true, out var parsedUnit))
                parsedUnit = UnitType.Piezas;

            var item = Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new InventoryItem
                {
                    Id = GetNextId(),
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

        #endregion

        #region CONSUME 

        public void Consume(string name, decimal quantity)
        {
            var item = Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (item == null) return;

            item.Quantity -= quantity;
            item.LastUpdated = DateTime.Now;
        }

        public void ConsumeById(int id, decimal quantity)
        {
            var item = Items.FirstOrDefault(x => x.Id == id);
            if (item == null) return;

            item.Quantity -= quantity;
            item.LastUpdated = DateTime.Now;
        }

        #endregion

        #region GETTERS 

        public InventoryItem? GetItem(string name)
        {
            return Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public InventoryItem? GetItemById(int id)
        {
            return Items.FirstOrDefault(x => x.Id == id);
        }

        #endregion

        #region NEW METHODS 

        /// <summary>
        /// Returns the next available ID. (◕‿◕✿)
        /// </summary>
        public int GetNextId()
        {
            return Items.Count == 0 ? 1 : Items.Max(i => i.Id) + 1;
        }

        /// <summary>
        /// Removes an item by ID. (ಥ﹏ಥ)
        /// </summary>
        public void RemoveItem(int id)
        {
            var item = Items.FirstOrDefault(i => i.Id == id);
            if (item != null)
                Items.Remove(item);
        }

        /// <summary>
        /// Updates an existing item. (≧◡≦)
        /// </summary>
        public void UpdateItem(InventoryItem updated)
        {
            var item = Items.FirstOrDefault(i => i.Id == updated.Id);
            if (item == null) return;

            item.Name = updated.Name;
            item.Quantity = updated.Quantity;
            item.Unit = updated.Unit;
            item.CostPerUnit = updated.CostPerUnit;
            item.UnitsPerPackage = updated.UnitsPerPackage;
            item.PackagesPerBox = updated.PackagesPerBox;
            item.LastUpdated = updated.LastUpdated;
        }

        #endregion
    }
}
