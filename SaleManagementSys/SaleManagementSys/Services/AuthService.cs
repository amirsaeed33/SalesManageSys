using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SaleManagementSys.Data;
using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _db.Logins.AsNoTracking().AnyAsync(l => l.Username == request.Username))
                return new AuthResponse { Success = false, Message = "Username already exists." };
            if (await _db.Logins.AsNoTracking().AnyAsync(l => l.Email == request.Email))
                return new AuthResponse { Success = false, Message = "Email already registered." };

            var login = new Login
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow
            };
            _db.Logins.Add(login);
            await _db.SaveChangesAsync();

            var token = GenerateJwt(login.Id, login.Username);
            return new AuthResponse
            {
                Success = true,
                Token = token,
                UserId = login.Id,
                Username = login.Username,
                Message = "Registered successfully."
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _db.Logins.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Username == request.Username);
            if (user == null)
                return new AuthResponse { Success = false, Message = "Invalid username or password." };
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new AuthResponse { Success = false, Message = "Invalid username or password." };

            var token = GenerateJwt(user.Id, user.Username);
            return new AuthResponse
            {
                Success = true,
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                Message = "Login successful."
            };
        }

        private string GenerateJwt(int userId, string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not set")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = _config["Jwt:Issuer"] ?? "SaleManagementSys";
            var audience = _config["Jwt:Audience"] ?? "SaleManagementSys";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(_config.GetValue<double>("Jwt:ExpiryHours", 24)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
