using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;

        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment env)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var categories = await _categoryService.GetAllAsync();
            return View(new ProductIndexViewModel { Products = products, Categories = categories });
        }

        public async Task<IActionResult> Catalog()
        {
            const int pageSize = 8;
            var (items, totalCount) = await _productService.GetActiveProductsPagedAsync(0, pageSize);
            ViewBag.CatalogTotalCount = totalCount;
            ViewBag.CatalogHasMore = totalCount > pageSize;
            ViewBag.CatalogPageSize = pageSize;
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> CatalogPage(int page = 1, int pageSize = 8, string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 8;
            var skip = (page - 1) * pageSize;
            var (items, totalCount) = await _productService.GetActiveProductsPagedAsync(skip, pageSize, search);
            var hasMore = skip + items.Count < totalCount;
            ViewBag.HasMore = hasMore;
            ViewBag.NextPage = page + 1;
            return PartialView("_CatalogProductBatch", items);
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
                TempData["ProductCategoryId"] = product.CategoryId;
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
