namespace SaleManagementSys.Models
{
    public class DashboardViewModel
    {
        public List<DashboardCardViewModel> Cards { get; set; } = new();
        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}
