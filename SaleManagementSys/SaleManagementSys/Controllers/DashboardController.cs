using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISaleService _saleService;

        public DashboardController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var todaySales = await _saleService.GetTodaySalesAsync();
            var todayProfit = await _saleService.GetTodayProfitAsync();
            var totalProductsSold = await _saleService.GetTotalProductsSoldAsync();
            var sales = await _saleService.GetAllSalesAsync();

            var viewModel = new DashboardViewModel
            {
                TodaySales = todaySales,
                TodayProfit = todayProfit,
                TotalProducts = totalProductsSold,
                Sales = sales ?? new List<Sale>()
            };

            return View(viewModel);
        }
    }
}
