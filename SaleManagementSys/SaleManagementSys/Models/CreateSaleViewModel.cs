using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    /// <summary>
    /// View model for creating a sale. Uses detail items without Sale/SaleId to fix validation errors.
    /// </summary>
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

        // SaleDate is set automatically server-side (DateTime.Now), not user-editable
        public List<CreateSaleDetailViewModel> SaleDetails { get; set; } = new();
    }
}
