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

    public Task<Summary?> GetReviewSummaryAsync(int productId) =>
        db.Summaries.FirstOrDefaultAsync(s => s.ProductId == productId);

    public async Task StoreReviewSummaryAsync(int productId, string summary)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddDays(7);

        var existing = await db.Summaries.FirstOrDefaultAsync(s => s.ProductId == productId);

        if (existing is null)
        {
            db.Summaries.Add(new Summary
            {
                ProductId = productId,
                Content = summary,
                GeneratedAt = now,
                ExpiresAt = expiresAt
            });
        }
        else
        {
            existing.Content = summary;
            existing.GeneratedAt = now;
            existing.ExpiresAt = expiresAt;
        }

        await db.SaveChangesAsync();
    }
}