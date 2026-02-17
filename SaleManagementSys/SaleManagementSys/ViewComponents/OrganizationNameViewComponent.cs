using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Services;

namespace SaleManagementSys.ViewComponents
{
    public class OrganizationNameViewComponent : ViewComponent
    {
        public class OrganizationBrandViewModel
        {
            public string Name { get; set; } = "SaleManagementSys";
            public string? LogoUrl { get; set; }
            public bool ShowLogo { get; set; }
        }

        private readonly IOrganizationSettingsService _settings;

        public OrganizationNameViewComponent(IOrganizationSettingsService settings)
        {
            _settings = settings;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool showLogo = false)
        {
            var org = await _settings.GetAsync();
            var name = !string.IsNullOrWhiteSpace(org?.Name) ? org.Name : "SaleManagementSys";
            var model = new OrganizationBrandViewModel
            {
                Name = name,
                LogoUrl = org?.LogoUrl,
                ShowLogo = showLogo
            };
            return View(model);
        }
    }
}
