using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IFeedbackService
    {
        Task<(bool Success, string Message)> SubmitFeedbackAsync(int productId, int rating, string? comment, string? customerName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductFeedback>> GetFeedbackForProductAsync(int productId, int maxCount = 50, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<int, FeedbackSummary>> GetFeedbackSummaryByProductIdsAsync(IEnumerable<int> productIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<int, (int Likes, int Dislikes)>> GetReactionCountsForFeedbackIdsAsync(IEnumerable<int> feedbackIds, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message, int Likes, int Dislikes)> SubmitReactionAsync(int feedbackId, bool isLike, string userIdentifier, CancellationToken cancellationToken = default);
        Task<IReadOnlyDictionary<int, bool>> GetUserReactionsForFeedbackIdsAsync(string userIdentifier, IEnumerable<int> feedbackIds, CancellationToken cancellationToken = default);
    }
}
