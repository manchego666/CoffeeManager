using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;

namespace CoffeeManager.Models.Class
{
    internal class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Employee Employee { get; set; }
        public List<SaleDetail> Details { get; set; }
        public decimal Total { get; set; }

        public Sale()
        {
            Details = new List<SaleDetail>();
        }
    }
}

