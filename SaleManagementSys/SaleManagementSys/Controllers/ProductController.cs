using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class ProductController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly Data.ApplicationDbContext _context;

        public ProductController(ISaleService saleService, Data.ApplicationDbContext context)
        {
            _saleService = saleService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _saleService.GetAllProductsAsync();
            return View(products);
        }

        public IActionResult Create()
        {
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}
