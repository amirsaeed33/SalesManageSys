using Microsoft.EntityFrameworkCore;
using SaleManagementSys.Data;
using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;

        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> SubmitFeedbackAsync(int productId, int rating, string? comment, string? customerName, CancellationToken cancellationToken = default)
        {
            if (rating < 1 || rating > 5)
                return (false, "Please select a rating from 1 to 5 stars.");

            var productExists = await _context.Products.AsNoTracking().AnyAsync(p => p.Id == productId, cancellationToken);
            if (!productExists)
                return (false, "Product not found.");

            comment = (comment ?? "").Trim();
            if (comment.Length > 2000)
                comment = comment.Substring(0, 2000);
            customerName = (customerName ?? "").Trim();
            if (customerName.Length > 200)
                customerName = customerName.Substring(0, 200);

            var feedback = new ProductFeedback
            {
                ProductId = productId,
                Rating = rating,
                Comment = string.IsNullOrEmpty(comment) ? null : comment,
                CustomerName = string.IsNullOrEmpty(customerName) ? null : customerName,
                CreatedAt = DateTime.UtcNow
            };
            _context.ProductFeedbacks.Add(feedback);
            await _context.SaveChangesAsync(cancellationToken);

            return (true, "Thank you for your feedback!");
        }

        public async Task<IReadOnlyList<ProductFeedback>> GetFeedbackForProductAsync(int productId, int maxCount = 50, CancellationToken cancellationToken = default)
        {
            if (maxCount < 1 || maxCount > 100) maxCount = 50;
            return await _context.ProductFeedbacks
                .AsNoTracking()
                .Where(f => f.ProductId == productId)
                .OrderByDescending(f => f.CreatedAt)
                .Take(maxCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyDictionary<int, FeedbackSummary>> GetFeedbackSummaryByProductIdsAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default)
        {
            var ids = productIds?.Distinct().ToList() ?? new List<int>();
            if (ids.Count == 0)
                return new Dictionary<int, FeedbackSummary>();

            var list = await _context.ProductFeedbacks
                .AsNoTracking()
                .Where(f => ids.Contains(f.ProductId))
                .GroupBy(f => f.ProductId)
                .Select(g => new { ProductId = g.Key, AverageRating = Math.Round(g.Average(f => f.Rating), 1), ReviewCount = g.Count() })
                .ToListAsync(cancellationToken);

            return list.ToDictionary(x => x.ProductId, x => new FeedbackSummary { AverageRating = x.AverageRating, ReviewCount = x.ReviewCount });
        }

        public async Task<IReadOnlyDictionary<int, (int Likes, int Dislikes)>> GetReactionCountsForFeedbackIdsAsync(IEnumerable<int> feedbackIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var ids = feedbackIds?.Distinct().ToList() ?? new List<int>();
                if (ids.Count == 0)
                    return new Dictionary<int, (int, int)>();

                var list = await _context.FeedbackReactions
                    .AsNoTracking()
                    .Where(r => ids.Contains(r.ProductFeedbackId))
                    .GroupBy(r => r.ProductFeedbackId)
                    .Select(g => new
                    {
                        FeedbackId = g.Key,
                        Likes = g.Count(r => r.IsLike),
                        Dislikes = g.Count(r => !r.IsLike)
                    })
                    .ToListAsync(cancellationToken);

                return list.ToDictionary(x => x.FeedbackId, x => (x.Likes, x.Dislikes));
            }
            catch (Exception ex)
            {
                return new Dictionary<int, (int Likes, int Dislikes)>();
            }
        }

        public async Task<(bool Success, string Message, int Likes, int Dislikes)> SubmitReactionAsync(int feedbackId, bool isLike, string userIdentifier, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userIdentifier))
                return (false, "Invalid session.", 0, 0);

            var feedbackExists = await _context.ProductFeedbacks.AsNoTracking().AnyAsync(f => f.Id == feedbackId, cancellationToken);
            if (!feedbackExists)
                return (false, "Review not found.", 0, 0);

            var existing = await _context.FeedbackReactions
                .FirstOrDefaultAsync(r => r.ProductFeedbackId == feedbackId && r.UserIdentifier == userIdentifier, cancellationToken);

            if (existing != null)
            {
                existing.IsLike = isLike;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _context.FeedbackReactions.Add(new FeedbackReaction
                {
                    ProductFeedbackId = feedbackId,
                    IsLike = isLike,
                    UserIdentifier = userIdentifier.Trim(),
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync(cancellationToken);

            var counts = await GetReactionCountsForFeedbackIdsAsync(new[] { feedbackId }, cancellationToken);
            var (likes, dislikes) = counts.TryGetValue(feedbackId, out var c) ? c : (0, 0);
            return (true, "Thank you!", likes, dislikes);
        }
    }
}
