namespace SaleManagementSys.Models
{
    public class ProductIndexViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }
}
