using server.Services;

namespace server.Endpoints;

public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this WebApplication app)
    {
        app.MapGet("/api/products/{id:int}/reviews", async (int id, ReviewService reviewService) =>
        {
            try
            {
                var (reviews, summary) = await reviewService.GetReviewsAsync(id);
                return Results.Json(new { summary, reviews });
            }
            catch (ArgumentException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
        });

        app.MapPost("/api/products/{id:int}/reviews/summarize", async (int id, ReviewService reviewService) =>
        {
            var summary = await reviewService.SummarizeReviewsAsync(id);
            return Results.Json(new { summary });
        });
    }
}
