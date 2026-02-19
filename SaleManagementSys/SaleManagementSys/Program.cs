using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using SaleManagementSys.Data;
using SaleManagementSys.Services;

var builder = WebApplication.CreateBuilder(args);

// When behind a reverse proxy (e.g. ngrok), trust forwarded headers so the app sees the real host/scheme
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// In production, listen on all interfaces (0.0.0.0) so the app is reachable from other machines/internet
if (builder.Environment.IsProduction())
{
    var urls = builder.Configuration["Urls"] ?? "http://0.0.0.0:5000";
    builder.WebHost.UseUrls(urls);
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register ApplicationDbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrganizationSettingsService, OrganizationSettingsService>();

var app = builder.Build();

// Use forwarded headers first (required for ngrok / reverse proxies)
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
