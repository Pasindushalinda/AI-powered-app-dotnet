using server.Services;

namespace server.Endpoints;

public static class ReviewEndpoints
{
    public static void MapReviewEndpoints(this WebApplication app)
    {
        app.MapGet("/api/products/{id:int}/reviews", async (int id, ReviewService reviewService) =>
        {
            var reviews = await reviewService.GetReviewsAsync(id);
            return Results.Json(reviews);
        });
    }
}
