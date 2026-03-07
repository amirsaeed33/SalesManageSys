using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    /// <summary>Like or dislike on a product feedback/review.</summary>
    public class FeedbackReaction
    {
        public int Id { get; set; }

        [Required]
        public int ProductFeedbackId { get; set; }

        /// <summary>True = like, false = dislike.</summary>
        public bool IsLike { get; set; }

        [Required]
        [StringLength(256)]
        public string UserIdentifier { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}
