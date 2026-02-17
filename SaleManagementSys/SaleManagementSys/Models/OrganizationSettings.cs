using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    public class OrganizationSettings
    {
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "Organization Name")]
        public string? Name { get; set; }

        [StringLength(500)]
        [Display(Name = "Logo")]
        public string? LogoUrl { get; set; }

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}
