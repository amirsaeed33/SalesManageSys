using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    public class CreateSaleViewModel
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(200)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = "walking customer";

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; } = "03321418639";

        [StringLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        public List<CreateSaleDetailViewModel> SaleDetails { get; set; } = new();
    }
}
