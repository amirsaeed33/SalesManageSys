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
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;

        public SaleController(ISaleService saleService, IProductService productService, ApplicationDbContext context)
        {
            _saleService = saleService;
            _productService = productService;
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
                return NotFound();
            return View(sale);
        }

        // GET: Sale/Create
        public async Task<IActionResult> Create()
        {
            var products = await _productService.GetActiveProductsAsync();
            ViewBag.Products = products;

            var model = new CreateSaleViewModel
            {
                SaleDate = DateTime.Today
            };
            return View(model);
        }

        // POST: Sale/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSaleViewModel model)
        {
            if (model.SaleDetails == null)
                model.SaleDetails = new List<CreateSaleDetailViewModel>();

            var validDetails = model.SaleDetails
                .Where(d => d.ProductId > 0 && d.Quantity > 0 && d.SalePrice > 0)
                .ToList();

            if (validDetails.Count == 0)
                ModelState.AddModelError("SaleDetails", "At least one product line is required (Product, Quantity > 0, Sale Price > 0).");

            if (ModelState.IsValid)
            {
                try
                {
                    var sale = new Sale
                    {
                        CustomerName = model.CustomerName,
                        PhoneNumber = model.PhoneNumber,
                        SaleDate = model.SaleDate,
                        TotalAmount = validDetails.Sum(d => d.SalePrice * d.Quantity),
                        TotalProfit = validDetails.Sum(d => (d.SalePrice - d.PurchasePrice) * d.Quantity)
                    };

                    _context.Sales.Add(sale);
                    await _context.SaveChangesAsync();

                    foreach (var d in validDetails)
                    {
                        _context.SaleDetails.Add(new SaleDetail
                        {
                            SaleId = sale.Id,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            SalePrice = d.SalePrice,
                            PurchasePrice = d.PurchasePrice
                        });
                    }
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Dashboard");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while saving the sale: {ex.Message}");
                }
            }

            ViewBag.Products = await _productService.GetActiveProductsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _saleService.DeleteSaleAsync(id);
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
