using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents the internal warehouse of the coffee shop.
    /// Stores consumables, tools and supplies. (✧ω✧) ZORRODEV2026
    /// </summary>
    internal class Warehouse
    {
        /// <summary>
        /// List of all inventory items in the warehouse. (≧◡≦)
        /// </summary>
        public List<InventoryItem> Items { get; set; } = new();

        /// <summary>
        /// Adds a new item or increases quantity if it already exists. (ง'̀-'́)ง
        /// </summary>
        public void AddItem(string name, decimal quantity, string unit)
        {
            var item = Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                Items.Add(new InventoryItem
                {
                    Id = Items.Count + 1,
                    Name = name,
                    Quantity = quantity,
                    Unit = unit,
                    LastUpdated = DateTime.Now
                });
            }
            else
            {
                item.Quantity += quantity;
                item.LastUpdated = DateTime.Now;
            }
        }

        /// <summary>
        /// Consumes quantity from an item. Can go negative. (ಠ‿ಠ)
        /// </summary>
        public void Consume(string name, decimal quantity)
        {
            var item = Items.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
    }
}
