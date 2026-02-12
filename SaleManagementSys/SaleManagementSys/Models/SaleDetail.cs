using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleManagementSys.Models
{
    public class SaleDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sale")]
        public int SaleId { get; set; }

        // Navigation property
        [ForeignKey(nameof(SaleId))]
        public virtual Sale Sale { get; set; } = null!;

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Purchase Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be greater than or equal to 0")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Sale Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Sale price must be greater than or equal to 0")]
        public decimal SalePrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
