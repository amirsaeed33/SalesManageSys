using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface ISaleService
    {
        Task<List<Sale>> GetAllSalesAsync();
        Task<List<Sale>> GetTodaySalesForDisplayAsync();
        Task<Sale?> GetSaleByIdAsync(int id);
        Task AddSaleAsync(Sale sale);
        Task<decimal> GetTodaySalesAsync();
        Task<decimal> GetTodayProfitAsync();
        Task<int> GetTodayProductsSoldAsync();
        Task<List<DailySaleSummary>> GetLast7DaysSalesAsync();
        Task<bool> DeleteSaleAsync(int id);
    }
}
