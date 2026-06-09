using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents the in-memory store of the system.
    /// Holds products, employees, sales, warehouse and financial data. (ง'̀-'́)ง ZORRODEV2026
    /// </summary>
    internal class Store
    {
        #region DATA (≧◡≦)

        /// <summary>
        /// List of products available in the store. (≧◡≦)
        /// </summary>
        public List<Product> Products { get; set; } = new();

        /// <summary>
        /// List of employees working in the store. (≧◡≦)
        /// </summary>
        public List<Employee> Employees { get; set; } = new();

        /// <summary>
        /// List of sales performed in the store. (≧◡≦)
        /// </summary>
        public List<Sale> Sales { get; set; } = new();

        /// <summary>
        /// Internal warehouse of the store. (≧◡≦)
        /// </summary>
        public Warehouse Warehouse { get; set; } = new();

        #endregion


        #region FINANCIAL DATA (✧ω✧)

        /// <summary>
        /// Total revenue generated from all sales (sum of sale totals). (≧◡≦)
        /// </summary>
        public decimal TotalRevenue { get; private set; }

        /// <summary>
        /// Total cost of producing all sold products. (≧◡≦)
        /// </summary>
        public decimal TotalCost { get; private set; }

        /// <summary>
        /// Total profit = Revenue - Cost. (≧◡≦)
        /// </summary>
        public decimal TotalProfit => TotalRevenue - TotalCost;

        /// <summary>
        /// Total losses (stolen items, unpaid orders, etc.). (╥﹏╥)
        /// </summary>
        public decimal TotalLosses { get; private set; }

        /// <summary>
        /// Total waste (expired milk, spilled coffee, etc.). (╥﹏╥)
        /// </summary>
        public decimal TotalWaste { get; private set; }

        #endregion


        #region PRODUCT METHODS (ง'̀-'́)ง

        /// <summary>
        /// Adds a new product to the store. (≧◡≦)
        /// </summary>
        public void AddProduct(Product product)
        {
            Products.Add(product);
        }

        /// <summary>
        /// Removes a product by ID. (ಠ‿ಠ)
        /// </summary>
        public bool RemoveProduct(int id)
        {
            var p = Products.FirstOrDefault(x => x.Id == id);
            if (p == null) return false;

            Products.Remove(p);
            return true;
        }

        /// <summary>
        /// Returns a product by ID. (◕‿◕✿)
        /// </summary>
        public Product? GetProduct(int id)
        {
            return Products.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Returns a product by name. (✧ω✧)
        /// </summary>
        public Product? GetProductByName(string name)
        {
            return Products.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion


        #region EMPLOYEE METHODS (≧◡≦)

        /// <summary>
        /// Adds a new employee to the store. (✿◠‿◠)
        /// </summary>
        public void AddEmployee(Employee employee)
        {
            Employees.Add(employee);
        }

        /// <summary>
        /// Removes an employee by ID. (⁀ᗢ⁀)
        /// </summary> 
        public bool RemoveEmployee(int id)
        {
            var e = Employees.FirstOrDefault(x => x.Id == id);
            if (e == null) return false;

            Employees.Remove(e);
            return true;
        }

        /// <summary>
        /// Returns an employee by ID. (◕‿◕✿)
        /// </summary>
        public Employee? GetEmployee(int id)
        {
            return Employees.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Returns an employee by full name. (✧ω✧)
        /// </summary>
        public Employee? GetEmployeeByName(string fullName)
        {
            return Employees.FirstOrDefault(x => x.GetFullName().Equals(fullName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion


        #region SALES METHODS (ง'̀-'́)ง

        /// <summary>
        /// Adds a new sale to the store and updates financial data. (≧◡≦)
        /// </summary>
        public void AddSale(Sale sale)
        {
            Sales.Add(sale);

            // 1. Revenue
            TotalRevenue += sale.Total;

            // 2. Cost of producing this sale
            decimal saleCost = 0m;

            foreach (var detail in sale.Details)
            {
                var product = detail.Product;
                if (product == null) continue;

                saleCost += product.GetUnitCost(Warehouse) * detail.Quantity;
            }

            TotalCost += saleCost;
        }

        /// <summary>
        /// Returns a sale by ID. (ಠ‿ಠ)
        /// </summary>
        public Sale? GetSale(int id)
        {
            return Sales.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Returns all sales made on a specific date. (◕‿◕✿)
        /// </summary>
        public List<Sale> GetSalesByDate(DateTime date)
        {
            return Sales.Where(x => x.Date.Date == date.Date).ToList();
        }

        #endregion


        #region LOSS & WASTE (╥﹏╥)

        /// <summary>
        /// Registers waste (merma) and updates warehouse + financials. (╥﹏╥)
        /// </summary>
        public void RegisterWaste(string ingredientName, decimal quantity)
        {
            var item = Warehouse.GetItem(ingredientName);
            if (item == null) return;

            item.Quantity -= quantity;
            TotalWaste += quantity * item.CostPerUnit;
        }

        /// <summary>
        /// Registers a financial loss (stolen items, unpaid orders, etc.). (╥﹏╥)
        /// </summary>
        public void RegisterLoss(decimal amount)
        {
            TotalLosses += amount;
        }

        #endregion


        #region UTILITIES (✧ω✧)

        public bool HasProducts() => Products.Count > 0;
        public bool HasEmployees() => Employees.Count > 0;
        public bool HasSales() => Sales.Count > 0;

        #endregion
    }
}
