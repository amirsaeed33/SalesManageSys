using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    /// <summary>
    /// View model for a single line item when creating a sale. No SaleId/Sale to avoid "The Sale field is required" validation.
    /// </summary>
    public class CreateSaleDetailViewModel
    {
        [Required(ErrorMessage = "Product is required")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale price must be greater than 0")]
        [Display(Name = "Sale Price")]
        public decimal SalePrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be >= 0")]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }
    }
}
