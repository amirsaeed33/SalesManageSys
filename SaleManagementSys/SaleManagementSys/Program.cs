using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SaleManagementSys.Data;
using SaleManagementSys.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection; 

var builder = WebApplication.CreateBuilder(args);

// 🔴 FIX 1: Add Data Protection for cookie correlation
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "Keys")))
    .SetApplicationName("SaleManagementSys");

// When behind a reverse proxy (e.g. ngrok), trust forwarded headers so the app sees the real host/scheme
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 🔴 FIX 2: Comment out or modify production URL configuration
// In production, listen on all interfaces but don't override in development
if (builder.Environment.IsProduction())
{
    var urls = builder.Configuration["Urls"] ?? "http://0.0.0.0:5000";
    builder.WebHost.UseUrls(urls);
}

// 🔴 FIX 3: Configure Cookie Policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.Secure = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Require login for all pages by default; use [AllowAnonymous] only for Login and auth API.
    options.Filters.Add(new AuthorizeFilter());
});


var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Default challenge for external logins
})
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;

        // 🔴 FIX 4: Cookie settings for development
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.Scope.Add("profile");
        options.Scope.Add("email");

        // Save tokens for later use if needed
        options.SaveTokens = true;

        // Map additional claims
        options.ClaimActions.MapJsonKey("picture", "picture");

    });
}

// JWT Authentication (existing code)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "";
if (!string.IsNullOrEmpty(jwtKey))
{
    authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });
}

// Register ApplicationDbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("SaleManagementSys")));

// Register services
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrganizationSettingsService, OrganizationSettingsService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmailVerified", policy =>
        policy.RequireClaim("email_verified", "true"));
});

var app = builder.Build();

// Use forwarded headers first (required for ngrok / reverse proxies)
app.UseForwardedHeaders();

// 🔴 FIX 6: Add cookie policy middleware
app.UseCookiePolicy();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Auth API: /auth/register, /auth/login, /auth/google, /auth/google-callback
app.MapControllerRoute(
    name: "dashboard",
    pattern: "Dashboard",
    defaults: new { controller = "Dashboard", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();