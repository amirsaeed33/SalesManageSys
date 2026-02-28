using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IOrganizationSettingsService
    {
        Task<OrganizationSettings?> GetAsync();
        Task SaveAsync(OrganizationSettings settings);
    }
}
