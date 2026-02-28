using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetActiveProductsAsync();
        Task<(List<Product> Items, int TotalCount)> GetActiveProductsPagedAsync(int skip, int take, string? search = null);
        Task<Product?> GetProductByIdAsync(int id);
        Task SaveProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}
