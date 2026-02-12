using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleManagementSys.Data;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly ApplicationDbContext _context;

        public SaleController(ISaleService saleService, ApplicationDbContext context)
        {
            _saleService = saleService;
            _context = context;
        }

        // GET: Sale
        public async Task<IActionResult> Index()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return View(sales);
        }

        // GET: Sale/Print/5
        public async Task<IActionResult> Print(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        // GET: Sale/Create
        public IActionResult Create()
        {
            var sale = new Sale
            {
                SaleDate = DateTime.Today
            };
            return View(sale);
        }

        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sale sale)
        {
            // Ensure SaleDetails is initialized
            if (sale.SaleDetails == null)
            {
                sale.SaleDetails = new List<SaleDetail>();
            }

            // Filter out empty or invalid sale details and create a new list
            var validSaleDetails = sale.SaleDetails
                .Where(sd => sd != null && 
                             !string.IsNullOrWhiteSpace(sd.ProductName) && 
                             sd.Quantity > 0 && 
                             sd.SalePrice > 0)
                .ToList();

            // Validate that at least one sale detail exists
            if (!validSaleDetails.Any())
            {
                ModelState.AddModelError("SaleDetails", "At least one product is required.");
            }

            // Validate each sale detail
            for (int i = 0; i < validSaleDetails.Count; i++)
            {
                var detail = validSaleDetails[i];
                if (string.IsNullOrWhiteSpace(detail.ProductName))
                {
                    ModelState.AddModelError($"SaleDetails[{i}].ProductName", "Product name is required.");
                }
                if (detail.Quantity <= 0)
                {
                    ModelState.AddModelError($"SaleDetails[{i}].Quantity", "Quantity must be greater than 0.");
                }
                if (detail.SalePrice <= 0)
                {
                    ModelState.AddModelError($"SaleDetails[{i}].SalePrice", "Sale price must be greater than 0.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate TotalAmount as sum of all SubTotals (Quantity * SalePrice)
                    sale.TotalAmount = validSaleDetails.Sum(sd => sd.SalePrice * sd.Quantity);
                    
                    // Calculate TotalProfit
                    sale.TotalProfit = validSaleDetails.Sum(sd => (sd.SalePrice - sd.PurchasePrice) * sd.Quantity);

                    // Clear SaleDetails from sale object before saving Sale first
                    sale.SaleDetails = new List<SaleDetail>();

                    // Save Sale first
                    _context.Sales.Add(sale);
                    await _context.SaveChangesAsync();

                    // Now loop through SaleItems and assign SaleId
                    foreach (var saleDetail in validSaleDetails)
                    {
                        saleDetail.SaleId = sale.Id;
                        _context.SaleDetails.Add(saleDetail);
                    }

                    // Save all SaleDetails
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Dashboard");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving the sale: {ex.Message}");
                }
            }

            // Restore SaleDetails for display in case of validation errors
            sale.SaleDetails = validSaleDetails;

            // If we get here, there were validation errors
            return View(sale);
        }
    }
}
