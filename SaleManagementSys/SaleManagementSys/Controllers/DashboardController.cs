using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Helpers;
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
            var todayItemsSold = await _saleService.GetTodayProductsSoldAsync();

            var cards = new List<DashboardCardViewModel>
            {
                new() { Label = "Today Sales", Value = todaySales.ToString("C", CurrencyHelper.PkrCulture), IconCssClass = "fas fa-coins", GradientCssClass = "card-gradient-primary" },
                new() { Label = "Today Profit", Value = todayProfit.ToString("C", CurrencyHelper.PkrCulture), IconCssClass = "fas fa-chart-line", GradientCssClass = "card-gradient-success" },
                new() { Label = "Total Items Sold", Value = todayItemsSold.ToString(), IconCssClass = "fas fa-box", GradientCssClass = "card-gradient-warning" }
            };

            var viewModel = new DashboardViewModel
            {
                Cards = cards
            };

            return View(viewModel);
        }
    }
}
