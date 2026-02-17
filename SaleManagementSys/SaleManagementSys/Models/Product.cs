using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleManagementSys.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Purchase Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be >= 0")]
        public decimal DefaultPurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Sale Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Sale price must be >= 0")]
        public decimal? DefaultSalePrice { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Stock Quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be >= 0")]
        public int StockQuantity { get; set; }
    }
}
