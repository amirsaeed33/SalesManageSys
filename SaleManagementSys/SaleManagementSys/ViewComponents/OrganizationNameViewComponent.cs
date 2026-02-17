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
            /// <summary>First two letters of Name, uppercase (for avatar when no logo).</summary>
            public string Initials { get; set; } = "SM";
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
            var initials = name.Length >= 2
                ? char.ToUpperInvariant(name[0]).ToString() + char.ToUpperInvariant(name[1])
                : name.Length == 1 ? char.ToUpperInvariant(name[0]).ToString() : "SM";
            var model = new OrganizationBrandViewModel
            {
                Name = name,
                LogoUrl = org?.LogoUrl,
                ShowLogo = showLogo,
                Initials = initials
            };
            return View(model);
        }
    }
}
