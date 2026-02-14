using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface ISaleService
    {
        /// <summary>
        /// Gets all sales (for internal/admin use). Database retains all records.
        /// </summary>
        Task<List<Sale>> GetAllSalesAsync();

        /// <summary>
        /// Gets only today's sales for frontend display (Sales page, Dashboard).
        /// Filters by SaleDate date equals today, ignoring time part.
        /// </summary>
        Task<List<Sale>> GetTodaySalesForDisplayAsync();

        Task<Sale?> GetSaleByIdAsync(int id);
        Task AddSaleAsync(Sale sale);
        Task<decimal> GetTodaySalesAsync();
        Task<decimal> GetTodayProfitAsync();

        /// <summary>
        /// Gets total quantity of products sold today (for dashboard display).
        /// </summary>
        Task<int> GetTodayProductsSoldAsync();

        Task<bool> DeleteSaleAsync(int id);
    }
}
