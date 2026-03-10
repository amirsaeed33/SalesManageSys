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
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                LastName = request.LastName?.Trim() ?? string.Empty,
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                AuthProvider = "Local" // 🔴 NEW: Track auth provider
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
                Email = login.Email,
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
                Email = user.Email,
                Message = "Login successful."
            };
        }

        // 🔴 NEW: Google Authentication methods
        public async Task<Login> FindOrCreateGoogleUserAsync(string email, string name, string googleId)
        {
            // Check if user exists with this email
            var user = await _db.Logins.FirstOrDefaultAsync(l => l.Email == email);

            if (user != null)
            {
                // User exists, update Google ID and provider if not set
                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = googleId;
                    user.AuthProvider = "Google";
                    user.LastLoginAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
                return user;
            }

            // Create new user for Google login
            var newUser = new Login
            {
                Email = email,
                Username = GenerateUniqueUsername(email.Split('@')[0]),
                FirstName = name?.Split(' ').FirstOrDefault() ?? "",
                LastName = name?.Contains(' ') == true ? name.Substring(name.IndexOf(' ') + 1) : "",
                GoogleId = googleId,
                AuthProvider = "Google",
                IsEmailVerified = true, // Google emails are verified
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                // Set a random password since user will login via Google
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
            };

            _db.Logins.Add(newUser);
            await _db.SaveChangesAsync();

            return newUser;
        }

        public async Task<string> GenerateJwtTokenAsync(int userId, string username)
        {
            return GenerateJwt(userId, username);
        }

        // 🔴 NEW: Helper method to generate unique username
        private string GenerateUniqueUsername(string baseUsername)
        {
            var username = baseUsername;
            var counter = 1;

            while (_db.Logins.Any(l => l.Username == username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
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