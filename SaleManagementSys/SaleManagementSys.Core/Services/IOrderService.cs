using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<List<Order>> GetOrdersForDisplayAsync(bool pendingFirst = true);
        /// <summary>Gets the number of orders placed today (by OrderDate date).</summary>
        Task<int> GetTodayOrdersCountAsync();
        Task<Order> CreateOrderAsync(CreateOrderViewModel model);
        Task<(Sale? sale, string? errorMessage)> ProcessOrderAsync(int orderId);
    }
}
