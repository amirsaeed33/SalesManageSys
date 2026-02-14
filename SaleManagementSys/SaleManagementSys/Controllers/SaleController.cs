using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;

        public SaleController(ISaleService saleService, IProductService productService)
        {
            _saleService = saleService;
            _productService = productService;
        }

        // GET: Sale
        public async Task<IActionResult> Index()
        {
            var sales = await _saleService.GetTodaySalesForDisplayAsync();
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
            return View(new CreateSaleViewModel());
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
                        SaleDate = DateTime.Now,
                        SaleDetails = validDetails.Select(d => new SaleDetail
                        {
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            SalePrice = d.SalePrice,
                            PurchasePrice = d.PurchasePrice
                        }).ToList()
                    };

                    await _saleService.AddSaleAsync(sale);
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
