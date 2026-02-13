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
        [Display(Name = "Default Purchase Price")]
        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be >= 0")]
        public decimal DefaultPurchasePrice { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
