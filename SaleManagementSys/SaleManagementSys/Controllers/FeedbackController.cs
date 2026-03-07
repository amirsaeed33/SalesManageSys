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
            const string cookieName = "feedback_uid";
            var userIdentifier = Request.Cookies[cookieName];

            var list = await _feedbackService.GetFeedbackForProductAsync(productId, max, cancellationToken);
            var feedbackIds = list.Select(f => f.Id).ToList();
            var reactionCounts = feedbackIds.Count > 0
                ? await _feedbackService.GetReactionCountsForFeedbackIdsAsync(feedbackIds, cancellationToken)
                : new Dictionary<int, (int Likes, int Dislikes)>();
            var userReactions = !string.IsNullOrWhiteSpace(userIdentifier) && feedbackIds.Count > 0
                ? await _feedbackService.GetUserReactionsForFeedbackIdsAsync(userIdentifier, feedbackIds, cancellationToken)
                : new Dictionary<int, bool>();

            var items = list.Select(f =>
            {
                var (likes, dislikes) = reactionCounts.TryGetValue(f.Id, out var c) ? c : (0, 0);
                string? userReaction = null;
                if (userReactions.TryGetValue(f.Id, out var isLike)) userReaction = isLike ? "like" : "dislike";
                return new
                {
                    id = f.Id,
                    rating = f.Rating,
                    comment = f.Comment,
                    customerName = f.CustomerName,
                    createdAt = f.CreatedAt,
                    likes,
                    dislikes,
                    userReaction
                };
            }).ToList();

            var totalCount = items.Count;
            var averageRating = totalCount > 0 ? Math.Round(items.Average(x => x.rating), 1) : 0.0;
            return Json(new { items, averageRating, totalCount });
        }

        [HttpPost]
        public async Task<IActionResult> React(int feedbackId, bool isLike, CancellationToken cancellationToken = default)
        {
            const string cookieName = "feedback_uid";
            var userIdentifier = Request.Cookies[cookieName];
            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                userIdentifier = Guid.NewGuid().ToString("N");
                Response.Cookies.Append(cookieName, userIdentifier, new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Lax, MaxAge = TimeSpan.FromDays(365) });
            }

            var (success, message, likes, dislikes) = await _feedbackService.SubmitReactionAsync(feedbackId, isLike, userIdentifier, cancellationToken);
            var userReaction = success ? (isLike ? "like" : "dislike") : (string?)null;
            return Json(new { success, message, likes, dislikes, userReaction });
        }
    }
}
