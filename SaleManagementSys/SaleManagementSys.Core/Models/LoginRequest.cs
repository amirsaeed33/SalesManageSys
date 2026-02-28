using System.ComponentModel.DataAnnotations;

namespace SaleManagementSys.Models
{
    /// <summary>Request DTO for /auth/login.</summary>
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
