# Sale Management System – App Index

Quick reference so you can ask questions about this app. **Namespace:** `SaleManagementSys`.

---

## What the app is

- **ASP.NET Core 8** MVC web app
- **SQL Server** via Entity Framework Core 8
- **Sale Management**: create sales (header + line items), view list, print, dashboard with today’s totals

---

## Entry & config

| Item | Path | Notes |
|------|------|--------|
| Entry point | `Program.cs` | Registers DbContext (SQL Server), `ISaleService`/`SaleService`, MVC; default route: `Dashboard/Index` |
| Config | `appsettings.json` | Connection string key: `DefaultConnection` |

---

## Controllers & routes

| Controller | File | Main actions & routes |
|------------|------|------------------------|
| **DashboardController** | `Controllers/DashboardController.cs` | `GET /Dashboard`, `GET /Dashboard/Index` – today’s sales/profit, total products, recent sales list |
| **SaleController** | `Controllers/SaleController.cs` | `GET /Sale` – list; `GET /Sale/Create`, `POST /Sale/Create` – new sale; `GET /Sale/Print/{id}` – print view |
| **HomeController** | `Controllers/HomeController.cs` | `GET /Home/Index`, `GET /Home/Privacy`, `GET /Home/Error` |

---

## Services

| Service | File | Purpose |
|---------|------|---------|
| **ISaleService** | `Services/ISaleService.cs` | GetAllSalesAsync, GetSaleByIdAsync, AddSaleAsync, GetTodaySalesAsync, GetTodayProfitAsync, GetTotalProductsSoldAsync |
| **SaleService** | `Services/SaleService.cs` | Implements ISaleService using `ApplicationDbContext` |

---

## Data & models

| Item | Path | Purpose |
|------|------|---------|
| **ApplicationDbContext** | `Data/ApplicationDbContext.cs` | DbSets: `Sales`, `SaleDetails`; configures Sale/SaleDetail and one-to-many (cascade delete) |
| **Sale** | `Models/Sale.cs` | Id, CustomerName, PhoneNumber, Email, SaleDate, TotalAmount, TotalProfit; navigation `SaleDetails` |
| **SaleDetail** | `Models/SaleDetail.cs` | Id, SaleId, ProductName, PurchasePrice, SalePrice, Quantity, Description |
| **DashboardViewModel** | `Models/DashboardViewModel.cs` | TodaySales, TodayProfit, TotalProducts, Sales list |
| **ErrorViewModel** | `Models/ErrorViewModel.cs` | RequestId, ShowRequestId for error page |

---

## Views (Razor)

| View | Path | Shows |
|------|------|--------|
| Dashboard | `Views/Dashboard/Index.cshtml` | Dashboard with stats and sales |
| Sale list | `Views/Sale/Index.cshtml` | All sales |
| Create sale | `Views/Sale/Create.cshtml` | Form for new sale + line items |
| Print sale | `Views/Sale/Print.cshtml` | Print view for one sale |
| Home | `Views/Home/Index.cshtml`, `Views/Home/Privacy.cshtml` | Home, Privacy |
| Shared | `Views/Shared/_Layout.cshtml`, `Error.cshtml`, `_ValidationScriptsPartial.cshtml` | Layout, error page, validation scripts |

---

## Important behavior

- **Create sale (SaleController.Create POST):** Validates at least one line; filters empty lines; computes `TotalAmount` and `TotalProfit`; saves `Sale` first, then `SaleDetail`s with `SaleId`. Redirects to `Dashboard/Index` on success.
- **Dashboard:** Uses `GetTodaySalesAsync`, `GetTodayProfitAsync`, `GetTotalProductsSoldAsync`, `GetAllSalesAsync` to build `DashboardViewModel`.
- **Migrations:** Under `Migrations/` (e.g. `InitialCreate`); DbContext in `Data/ApplicationDbContext.cs`.

---

## Project layout (source)

```
SaleManagementSys/
├── SaleManagementSys.sln
└── SaleManagementSys/
    ├── Program.cs
    ├── appsettings.json
    ├── Controllers/     (Dashboard, Sale, Home)
    ├── Data/            (ApplicationDbContext)
    ├── Migrations/
    ├── Models/          (Sale, SaleDetail, DashboardViewModel, ErrorViewModel)
    ├── Services/        (ISaleService, SaleService)
    └── Views/           (Dashboard, Sale, Home, Shared)
```

Use this index to ask things like: “Where is the sale total calculated?”, “How do I add a new report?”, “Where is the dashboard data loaded?”, etc.
