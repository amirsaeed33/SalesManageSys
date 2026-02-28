using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(200)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        public List<CreateOrderDetailViewModel> OrderDetails { get; set; } = new();
    }

    public class CreateOrderDetailViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SalePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; }
    }
}
