using Microsoft.EntityFrameworkCore;
using SaleManagementSys.Data;
using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public class SaleService : ISaleService
    {
        private readonly ApplicationDbContext _context;

        public SaleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sale>> GetAllSalesAsync()
        {
            return await _context.Sales
                .AsNoTracking()
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .OrderByDescending(s => s.SaleDate)
                .ThenByDescending(s => s.Id)
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddSaleAsync(Sale sale)
        {
            // Ensure SaleDetails is initialized
            if (sale.SaleDetails == null)
            {
                sale.SaleDetails = new List<SaleDetail>();
            }

            // Ensure SaleDate is set before saving
            if (sale.SaleDate == default(DateTime) || sale.SaleDate == DateTime.MinValue)
            {
                sale.SaleDate = DateTime.Today;
            }

            // Calculate TotalAmount and TotalProfit before saving
            if (sale.SaleDetails.Any())
            {
                sale.TotalAmount = sale.SaleDetails.Sum(sd => sd.SalePrice * sd.Quantity);
                sale.TotalProfit = sale.SaleDetails.Sum(sd => (sd.SalePrice - sd.PurchasePrice) * sd.Quantity);
            }
            else
            {
                sale.TotalAmount = 0;
                sale.TotalProfit = 0;
            }

            // Add the sale first - EF Core will track SaleDetails through the navigation property
            _context.Sales.Add(sale);
            
            // Save changes - EF Core will automatically save SaleDetails because they're in the SaleDetails collection
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTodaySalesAsync()
        {
            return await _context.Sales
                .AsNoTracking()
                .Where(s => s.SaleDate.Date == DateTime.Today)
                .SumAsync(s => s.TotalAmount);
        }

        public async Task<decimal> GetTodayProfitAsync()
        {
            return await _context.Sales
                .AsNoTracking()
                .Where(s => s.SaleDate.Date == DateTime.Today)
                .SumAsync(s => s.TotalProfit);
        }

        public async Task<int> GetTotalProductsSoldAsync()
        {
            return await _context.SaleDetails
                .AsNoTracking()
                .SumAsync(sd => sd.Quantity);
        }
    }
}
