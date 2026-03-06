using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    public class ProductFeedback
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        /// <summary>Star rating 1-5.</summary>
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string? CustomerName { get; set; }
    }
}
