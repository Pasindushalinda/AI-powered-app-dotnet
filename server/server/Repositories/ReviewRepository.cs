using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;

namespace server.Repositories;

public class ReviewRepository(AppDbContext db) : IReviewRepository
{
    public Task<List<Review>> GetReviewsAsync(int productId, int? limit = null)
    {
        var query = db.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt);

        return (limit.HasValue ? query.Take(limit.Value) : query).ToListAsync();
    }
}