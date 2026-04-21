using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;

namespace server.Repositories;

public class ReviewRepository(AppDbContext db) : IReviewRepository
{
    public Task<List<Review>> GetReviewsAsync(int productId) =>
        db.Reviews
          .Where(r => r.ProductId == productId)
          .OrderByDescending(r => r.CreatedAt)
          .ToListAsync();
}