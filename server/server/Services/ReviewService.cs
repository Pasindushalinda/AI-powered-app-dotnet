using server.Domain;
using server.Repositories;

namespace server.Services;

public class ReviewService(IReviewRepository reviewRepository)
{
    public Task<List<Review>> GetReviewsAsync(int productId) =>
        reviewRepository.GetReviewsAsync(productId);
}