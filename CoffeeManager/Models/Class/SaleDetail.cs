using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeManager.Models.Class
{
    internal class SaleDetail
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public SaleDetail() { }
    }
}
