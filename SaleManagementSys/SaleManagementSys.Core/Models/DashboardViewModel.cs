namespace SaleManagementSys.Models
{
    public class DashboardViewModel
    {
        public List<DashboardCardViewModel> Cards { get; set; } = new();
        /// <summary>Display name for "Welcome back, {Username}". May be null if not authenticated server-side; view can use localStorage.</summary>
        public string? Username { get; set; }
        /// <summary>Last 7 days for chart: date labels (e.g. "Mon 24").</summary>
        public List<string> ChartLabels { get; set; } = new();
        /// <summary>Sales amount per day.</summary>
        public List<decimal> ChartSales { get; set; } = new();
        /// <summary>Profit amount per day.</summary>
        public List<decimal> ChartProfit { get; set; } = new();
    }
}
