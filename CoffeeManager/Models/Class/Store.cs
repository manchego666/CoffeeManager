using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeManager.Models.Class
{
    internal class Store
    {
        public string Name { get; set; }
        public List<Product> Products { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Sale> Sales { get; set; }

        public Store()
        {
            Products = new List<Product>();
            Employees = new List<Employee>();
            Sales = new List<Sale>();
        }
    }
}
    