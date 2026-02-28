using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task SaveAsync(Category category);
        Task<bool> DeleteAsync(int id);
    }
}
