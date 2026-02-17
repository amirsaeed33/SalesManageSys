using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<List<Order>> GetOrdersForDisplayAsync(bool pendingFirst = true);
        Task<Order> CreateOrderAsync(CreateOrderViewModel model);
        /// <summary>Converts order to sale, reduces product stock, marks order as Processed. Returns (sale, errorMessage); sale is null on failure.</summary>
        Task<(Sale? sale, string? errorMessage)> ProcessOrderAsync(int orderId);
    }
}
