using SaleManagementSys.Models;

namespace SaleManagementSys.Services
{
    public interface IFeedbackService
    {
        Task<(bool Success, string Message)> SubmitFeedbackAsync(int productId, int rating, string? comment, string? customerName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ProductFeedback>> GetFeedbackForProductAsync(int productId, int maxCount = 50, CancellationToken cancellationToken = default);
    }
}
