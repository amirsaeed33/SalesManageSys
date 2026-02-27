using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    /// <summary>Authentication service: register, login, BCrypt hashing, JWT generation.</summary>
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
