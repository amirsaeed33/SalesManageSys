using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    /// <summary>Authentication API. Routes: POST /auth/register, POST /auth/login.</summary>
    [ApiController]
    [Route("auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
                return BadRequest(new AuthResponse { Success = false, Message = "Invalid request." });
            var result = await _authService.RegisterAsync(request);
            if (!result.Success)
                return BadRequest(result);
            await SignInCookieAsync(result.UserId!.Value, result.Username ?? "");
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest(new AuthResponse { Success = false, Message = "Invalid request." });
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
                return Unauthorized(result);
            await SignInCookieAsync(result.UserId!.Value, result.Username ?? "");
            return Ok(result);
        }

        private async Task SignInCookieAsync(int userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                });
        }
    }
}
