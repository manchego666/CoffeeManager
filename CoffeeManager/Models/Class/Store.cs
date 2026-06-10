// ===============================================================
//  ZORRODEV 2026 — Store Engine
//  Authors: Christopher (≧◡≦), Daniel (ง'̀-'́)ง, Brayan (✧ω✧), Jesús (◕‿◕✿)
//  Description: In-memory store + financials + auto notifications.
// ===============================================================

using CoffeeManager.Services.Data;
using CoffeeManager.Services.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents the in-memory store of the system.
    /// Holds products, employees, sales, warehouse and financial data. (ง'̀-'́)ง ZORRODEV2026
    /// </summary>
    public class Store
    {
        private readonly NotificationService _notifications = new();

        #region DATA (≧◡≦)

        /// <summary>
        /// List of products available in the store. (≧◡≦)
        /// </summary>
        public List<Product> Products { get; set; }


        /// <summary>
        /// List of employees working in the store. (≧◡≦)
        /// </summary>
        public List<Employee> Employees { get; set; }

        /// <summary>
        /// List of sales performed in the store. (≧◡≦)
        /// </summary>
        public List<Sale> Sales { get; set; } = new();

        /// <summary>
        /// Internal warehouse of the store. (≧◡≦)
        /// </summary>
        public Warehouse Warehouse { get; set; } = new();

        #endregion


        public Store()
        {
            Employees = EmployeeService.Load() ?? new List<Employee>();
            Warehouse = WarehouseService.Load() ?? new Warehouse();
            Products = ProductService.Load() ?? new List<Product>();
            Sales = new List<Sale>();
        }



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
            product.Id = Products.Count + 1;
            Products.Add(product);
            ProductService.Save(Products);
            _notifications.NotifyProductCreated(product.Name);
        }


        /// <summary>
        /// Call this after editing a product to register an update notification. (✧ω✧)
        /// </summary>
        public void UpdateProduct(Product product)
        {
            ProductService.Save(Products);
            _notifications.NotifyProductUpdated(product.Name);
        }


        /// <summary>
        /// Removes a product by ID. (ಠ‿ಠ)
        /// </summary>
        public bool RemoveProduct(int id)
        {
            var p = Products.FirstOrDefault(x => x.Id == id);
            if (p == null) return false;

            Products.Remove(p);
            ProductService.Save(Products);
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
            EmployeeService.Save(Employees);
            _notifications.NotifyUserCreated(employee.GetFullName());
        }


        /// <summary>
        /// Call this after editing an employee to register an update notification. (✧ω✧)
        /// </summary>
        public void UpdateEmployee(Employee employee)
        {
            EmployeeService.Save(Employees);
            _notifications.NotifyUserUpdated(employee.GetFullName());
        }


        /// <summary>
        /// Removes an employee by ID. (⁀ᗢ⁀)
        /// </summary> 
        public bool RemoveEmployee(int id)
        {
            var e = Employees.FirstOrDefault(x => x.Id == id);
            if (e == null) return false;

            Employees.Remove(e);
            EmployeeService.Save(Employees);
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
        /// Also updates inventory and triggers stock notifications. (✧ω✧)
        /// </summary>
        public void AddSale(Sale sale)
        {
            Sales.Add(sale);

            decimal saleRevenue = 0m;
            decimal saleCost = 0m;

            foreach (var detail in sale.Details)
            {
                var product = detail.Product;
                if (product == null) continue;

                // 1) Ingresos
                var subtotal = product.Price * detail.Quantity;
                saleRevenue += subtotal;

                // 2) Consumo de producto / almacén
                //    y cálculo de costo real
                var warnings = product.PrepareAndConsume(Warehouse, detail.Quantity);


                // 3) Costo estimado por unidad * cantidad
                var unitCost = product.GetUnitCost(Warehouse);
                saleCost += unitCost * detail.Quantity;

                // 4) Notificaciones de stock bajo / agotado (producto)
                if (!product.UsesWarehouse)
                {
                    if (product.Stock <= 0)
                        _notifications.NotifyOutOfStock(product.Name);
                    else if (product.Stock <= 3) // umbral ejemplo
                        _notifications.NotifyLowStock(product.Name, product.Stock);
                }
            }

            TotalRevenue += saleRevenue;
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
        /// Also triggers stock notifications if needed. (✧ω✧)
        /// </summary>
        public void RegisterWaste(string ingredientName, decimal quantity)
        {
            var item = Warehouse.GetItem(ingredientName);
            if (item == null) return;

            item.Consume(quantity);
            TotalWaste += quantity * item.CostPerUnit;

            if (item.IsOutOfStock())
                _notifications.NotifyOutOfStock(item.Name);
            else if (item.IsLowStock())
                _notifications.NotifyLowStock(item.Name, item.Quantity);
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
