using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create() => RedirectToAction(nameof(Index));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Product product)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please fix the errors below.";
                TempData["ProductId"] = product.Id;
                TempData["ProductName"] = product.Name;
                TempData["ProductPrice"] = product.DefaultPurchasePrice;
                TempData["ProductDescription"] = product.Description ?? "";
                TempData["ProductIsActive"] = product.IsActive;
                return RedirectToAction(nameof(Index));
            }

            await _productService.SaveProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _productService.DeleteProductAsync(id))
                TempData["Message"] = "Cannot delete: product is used in one or more sales.";
            return RedirectToAction(nameof(Index));
        }
    }
}
