using Microsoft.EntityFrameworkCore;
using SaleManagementSys.Data;
using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public class OrganizationSettingsService : IOrganizationSettingsService
    {
        private readonly ApplicationDbContext _context;

        public OrganizationSettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrganizationSettings?> GetAsync()
        {
            return await _context.OrganizationSettings
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task SaveAsync(OrganizationSettings settings)
        {
            var existing = await _context.OrganizationSettings.FirstOrDefaultAsync();
            if (existing == null)
            {
                _context.OrganizationSettings.Add(settings);
            }
            else
            {
                existing.Name = settings.Name;
                existing.LogoUrl = settings.LogoUrl;
                existing.PhoneNumber = settings.PhoneNumber;
                existing.Address = settings.Address;
            }
            await _context.SaveChangesAsync();
        }
    }
}
