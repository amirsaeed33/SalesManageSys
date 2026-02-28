namespace SaleManagementSys.Models
{
    /// <summary>Response DTO for auth APIs (includes JWT and user info).</summary>
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
    }
}
