using server.Domain;

namespace server.Repositories;

public interface IReviewRepository
{
    Task<List<Review>> GetReviewsAsync(int productId, int? limit = null);
    Task<string?> GetReviewSummaryAsync(int productId);
    Task StoreReviewSummaryAsync(int productId, string summary);
}
