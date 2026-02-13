using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface ISaleService
    {
        Task<List<Sale>> GetAllSalesAsync();
        Task<Sale?> GetSaleByIdAsync(int id);
        Task AddSaleAsync(Sale sale);
        Task<decimal> GetTodaySalesAsync();
        Task<decimal> GetTodayProfitAsync();
        Task<int> GetTotalProductsSoldAsync();
    }
}
