namespace SaleManagementSys.Models
{
    public class DashboardCardViewModel
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string IconCssClass { get; set; } = "fas fa-circle";
        public string GradientCssClass { get; set; } = "card-gradient-primary";
    }
}
