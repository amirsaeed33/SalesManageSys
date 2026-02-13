using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetActiveProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        /// <summary>Adds when Id is 0, updates when Id &gt; 0.</summary>
        Task SaveProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}
