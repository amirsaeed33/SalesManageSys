using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class OrganizationSettingsController : Controller
    {
        private readonly IOrganizationSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public OrganizationSettingsController(IOrganizationSettingsService settingsService, IWebHostEnvironment env)
        {
            _settingsService = settingsService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetAsync();
            return View(settings ?? new OrganizationSettings());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(OrganizationSettings settings, IFormFile? LogoFile)
        {
            if (LogoFile != null && LogoFile.Length > 0)
            {
                var ext = Path.GetExtension(LogoFile.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(ext) || (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp"))
                    ext = ".png";
                var fileName = $"logo_{Guid.NewGuid():N}{ext}";
                var dir = Path.Combine(_env.WebRootPath, "images", "organization");
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    await LogoFile.CopyToAsync(stream);
                settings.LogoUrl = "/images/organization/" + fileName;
            }
            else
            {
                var existing = await _settingsService.GetAsync();
                if (existing != null && !string.IsNullOrEmpty(existing.LogoUrl))
                    settings.LogoUrl = existing.LogoUrl;
            }

            await _settingsService.SaveAsync(settings);
            TempData["AlertMessage"] = "Organization settings saved successfully.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
    }
}
