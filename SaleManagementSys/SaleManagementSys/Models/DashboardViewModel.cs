namespace SaleManagementSys.Models
{
    public class DashboardViewModel
    {
        public decimal TodaySales { get; set; }
        public decimal TodayProfit { get; set; }
        public int TotalProducts { get; set; }
        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}
