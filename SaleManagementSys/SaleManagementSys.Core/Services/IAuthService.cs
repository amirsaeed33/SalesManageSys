using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<Login> FindOrCreateGoogleUserAsync(string email, string name, string googleId);
        Task<string> GenerateJwtTokenAsync(int userId, string username);
    }
}
