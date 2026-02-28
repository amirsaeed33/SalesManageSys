using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Please enter a valid category name.";
                TempData["AlertType"] = "warning";
                return RedirectToAction(nameof(Index));
            }

            var isNew = category.Id == 0;
            await _categoryService.SaveAsync(category);
            TempData["AlertMessage"] = isNew ? "Category created successfully." : "Category updated successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _categoryService.DeleteAsync(id))
            {
                TempData["AlertMessage"] = "Cannot delete category (it may be used by products).";
                TempData["AlertType"] = "danger";
            }
            else
            {
                TempData["AlertMessage"] = "Category deleted successfully.";
                TempData["AlertType"] = "success";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

