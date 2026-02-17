using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Models;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersForDisplayAsync(pendingFirst: true);
            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            if (model.OrderDetails == null)
                model.OrderDetails = new List<CreateOrderDetailViewModel>();

            var validDetails = model.OrderDetails
                .Where(d => d.ProductId > 0 && d.Quantity > 0 && d.SalePrice > 0)
                .ToList();

            if (validDetails.Count == 0)
                ModelState.AddModelError("OrderDetails", "At least one product line is required.");

            if (ModelState.IsValid)
            {
                try
                {
                    await _orderService.CreateOrderAsync(model);
                    var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                    if (isAjax)
                        return Json(new { success = true });
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var isAjaxRequest = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjaxRequest)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new { key = x.Key, message = e.ErrorMessage ?? "Invalid value" }))
                    .ToList();
                return BadRequest(new { success = false, errors });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int id)
        {
            var (sale, errorMessage) = await _orderService.ProcessOrderAsync(id);
            if (sale != null)
                return RedirectToAction("Index", "Sale");
            TempData["OrderError"] = errorMessage ?? "Failed to process order.";
            return RedirectToAction(nameof(Index));
        }
    }
}
