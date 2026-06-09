namespace CoffeeManager.Models.Class
{
    public class DailyReport
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalTransactions { get; set; }
        public int Employees { get; set; }
        public int Products { get; set; }
    }
}
