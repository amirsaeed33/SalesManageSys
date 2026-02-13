using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleManagementSys.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(200)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Sale Date")]
        public DateTime SaleDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Profit")]
        [Range(0, double.MaxValue, ErrorMessage = "Total profit must be greater than or equal to 0")]
        public decimal TotalProfit { get; set; }

        // Navigation property
        public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
