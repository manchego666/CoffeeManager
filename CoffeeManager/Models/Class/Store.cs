using System;
using System.Collections.Generic;
using System.Linq;
using CoffeeManager.Models.Class;

namespace CoffeeManager.Models.Class
{
    /// <summary>
    /// Represents the in-memory store of the system.
    /// Holds products, employees and sales for runtime operations. (ง'̀-'́)ง ZORRODEV2026
    /// </summary>
    internal class Store
    {
        #region DATA (≧◡≦)

        /// <summary>
        /// List of all products in the store. (✿◠‿◠)
        /// </summary>
        public List<Product> Products { get; set; } = new();

        /// <summary>
        /// List of all employees registered in the store. (◕‿◕✿)
        /// </summary>
        public List<Employee> Employees { get; set; } = new();

        /// <summary>
        /// List of all sales made in the store. (✧ω✧)
        /// </summary>
        public List<Sale> Sales { get; set; } = new();

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
        /// Adds a new sale to the store. (≧◡≦)
        /// </summary>
        public void AddSale(Sale sale)
        {
            Sales.Add(sale);
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

        #region UTILITIES (✧ω✧)

        /// <summary>
        /// Returns true if the store has products. (≧◡≦)
        /// </summary>
        public bool HasProducts() => Products.Count > 0;

        /// <summary>
        /// Returns true if the store has employees. (◕‿◕✿)
        /// </summary>
        public bool HasEmployees() => Employees.Count > 0;

        /// <summary>
        /// Returns true if the store has sales. (ಠ‿ಠ)
        /// </summary>
        public bool HasSales() => Sales.Count > 0;

        #endregion
    }
}
