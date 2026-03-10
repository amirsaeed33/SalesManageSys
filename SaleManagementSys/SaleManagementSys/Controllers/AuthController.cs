using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    /// <summary>Authentication API. Routes: POST /auth/register, POST /auth/login, GET /auth/google, GET /auth/google-callback</summary>
    [ApiController]
    [Route("auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
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

        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                // 🔴 FIX 1: Use absolute URL for redirect
                RedirectUri = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme)
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()  // 🔴 FIX 2: Change return type to IActionResult
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Succeeded != true)
            {
                _logger.LogWarning("Google authentication failed");

                // 🔴 FIX 3: Redirect to login page with error
                return Redirect("/Home/Login?googleAuth=error");
            }

            try
            {
                // Extract user information from Google claims
                var email = result.Principal?.FindFirstValue(ClaimTypes.Email);
                var name = result.Principal?.FindFirstValue(ClaimTypes.Name);
                var googleId = result.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
                {
                    return Redirect("/Home/Login?googleAuth=error");
                }

                // Check if user exists in database, if not create new user
                var user = await _authService.FindOrCreateGoogleUserAsync(email, name, googleId);

                if (user == null)
                {
                    return Redirect("/Home/Login?googleAuth=error");
                }

                // Sign in the user
                await SignInCookieAsync(user.Id, user.Username);

                // 🔴 FIX 4: Generate token and store in cookie/session if needed
                var token = await _authService.GenerateJwtTokenAsync(user.Id, user.Username);

                // Store token in cookie or response
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                // 🔴 FIX 5: Redirect to dashboard with success flag
                return Redirect("/Dashboard/Index?googleAuth=success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Google callback");
                return Redirect("/Home/Login?googleAuth=error");
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("AuthToken");
            return Ok(new { Success = true, Message = "Logged out successfully" });
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