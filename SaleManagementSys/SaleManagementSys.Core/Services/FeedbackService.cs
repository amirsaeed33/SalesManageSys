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
    }
}
