using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductService productService, IWebHostEnvironment env)
        {
            _productService = productService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Catalog()
        {
            var products = await _productService.GetActiveProductsAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create() => RedirectToAction(nameof(Index));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Product product, IFormFile? ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp"))
                    ext = ".jpg";
                var fileName = $"{Guid.NewGuid():N}{ext}";
                var dir = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    await ImageFile.CopyToAsync(stream);
                product.ImageUrl = "/images/products/" + fileName;
            }

            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Please fix the errors below.";
                TempData["AlertType"] = "warning";
                TempData["ProductId"] = product.Id;
                TempData["ProductName"] = product.Name;
                TempData["ProductPrice"] = product.DefaultPurchasePrice;
                TempData["ProductDefaultSalePrice"] = product.DefaultSalePrice;
                TempData["ProductStockQuantity"] = product.StockQuantity;
                TempData["ProductImageUrl"] = product.ImageUrl ?? "";
                TempData["ProductDescription"] = product.Description ?? "";
                TempData["ProductIsActive"] = product.IsActive;
                return RedirectToAction(nameof(Index));
            }

            await _productService.SaveProductAsync(product);
            TempData["AlertMessage"] = "Product saved successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _productService.DeleteProductAsync(id))
            {
                TempData["AlertMessage"] = "Cannot delete: product is used in one or more sales.";
                TempData["AlertType"] = "danger";
            }
            else
            {
                TempData["AlertMessage"] = "Product deleted successfully.";
                TempData["AlertType"] = "success";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
