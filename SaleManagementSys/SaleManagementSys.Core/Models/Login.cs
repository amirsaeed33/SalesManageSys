using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    public class Login
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        
        [StringLength(100)]
        public string? GoogleId { get; set; }

        [StringLength(20)]
        public string? AuthProvider { get; set; } = "Local"; // "Local" or "Google"

        public bool IsEmailVerified { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
