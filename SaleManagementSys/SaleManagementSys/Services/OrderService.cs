using Microsoft.EntityFrameworkCore;
using SaleManagementSys.Data;
using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISaleService _saleService;

        public OrderService(ApplicationDbContext context, ISaleService saleService)
        {
            _context = context;
            _saleService = saleService;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetOrdersForDisplayAsync(bool pendingFirst = true)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .AsNoTracking();
            if (pendingFirst)
                query = query.OrderByDescending(o => o.Status == OrderStatus.Pending).ThenByDescending(o => o.OrderDate).ThenByDescending(o => o.Id);
            else
                query = query.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id);
            return await query.ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(CreateOrderViewModel model)
        {
            var validDetails = (model.OrderDetails ?? new List<CreateOrderDetailViewModel>())
                .Where(d => d.ProductId > 0 && d.Quantity > 0 && d.SalePrice > 0)
                .ToList();
            decimal total = validDetails.Sum(d => d.SalePrice * d.Quantity);
            var order = new Order
            {
                CustomerName = model.CustomerName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                OrderDate = DateTime.Now,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                OrderDetails = validDetails.Select(d => new OrderDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    SalePrice = d.SalePrice,
                    PurchasePrice = d.PurchasePrice
                }).ToList()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<(Sale? sale, string? errorMessage)> ProcessOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return (null, "Order not found.");
            if (order.Status != OrderStatus.Pending)
                return (null, "Order is already processed or cancelled.");
            foreach (var od in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(od.ProductId);
                if (product == null) return (null, $"Product not found (id {od.ProductId}).");
                if (product.StockQuantity < od.Quantity)
                    return (null, $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}, requested: {od.Quantity}.");
            }
            var sale = new Sale
            {
                CustomerName = order.CustomerName,
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                SaleDate = DateTime.Now,
                SaleDetails = order.OrderDetails.Select(od => new SaleDetail
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    SalePrice = od.SalePrice,
                    PurchasePrice = od.PurchasePrice
                }).ToList()
            };
            await _saleService.AddSaleAsync(sale);
            foreach (var od in order.OrderDetails)
            {
                od.Product.StockQuantity -= od.Quantity;
            }
            order.Status = OrderStatus.Processed;
            await _context.SaveChangesAsync();
            return (sale, null);
        }
    }
}
