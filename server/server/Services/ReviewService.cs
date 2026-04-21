using server.Domain;
using server.Llm;
using server.Repositories;

namespace server.Services;

public class ReviewService(IReviewRepository reviewRepository, LlmClient llmClient)
{
    public Task<List<Review>> GetReviewsAsync(int productId) =>
        reviewRepository.GetReviewsAsync(productId);

    public async Task<string> SummarizeReviewsAsync(int productId)
    {
        var reviews = await reviewRepository.GetReviewsAsync(productId, limit: 10);
        var joinedReviews = string.Join("\n\n", reviews.Select(r => r.Content));

        var prompt = $"""
            Summarize the following customer reviews into a short paragraph
            highlighting key themes, both positive and negative:

            {joinedReviews}
            """;

        var result = await llmClient.GenerateTextAsync(new GenerateTextOptions
        {
            Prompt = prompt,
            Temperature = 0.2f,
            MaxTokens = 500
        });

        return result.Text;
    }
}
