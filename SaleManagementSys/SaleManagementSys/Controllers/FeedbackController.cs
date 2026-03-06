using Microsoft.AspNetCore.Mvc;
using SaleManagementSys.Services;

namespace SaleManagementSys.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int productId, int rating, string? comment, string? customerName, CancellationToken cancellationToken = default)
        {
            var (success, message) = await _feedbackService.SubmitFeedbackAsync(productId, rating, comment, customerName, cancellationToken);
            return Json(new { success, message });
        }

        [HttpGet]
        public async Task<IActionResult> GetByProduct(int productId, int max = 50, CancellationToken cancellationToken = default)
        {
            var list = await _feedbackService.GetFeedbackForProductAsync(productId, max, cancellationToken);
            var items = list.Select(f => new
            {
                rating = f.Rating,
                comment = f.Comment,
                customerName = f.CustomerName,
                createdAt = f.CreatedAt
            }).ToList();
            var totalCount = items.Count;
            var averageRating = totalCount > 0 ? Math.Round(items.Average(x => x.rating), 1) : 0.0;
            return Json(new { items, averageRating, totalCount });
        }
    }
}
