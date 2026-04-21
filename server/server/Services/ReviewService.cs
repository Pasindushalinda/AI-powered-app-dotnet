using Microsoft.Extensions.AI;
using server.Domain;
using server.Models;
using server.Repositories;

namespace server.Services;

public class ReviewService(IReviewRepository reviewRepository, IChatClient chatClient, LlmOptions llmOptions)
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

        var response = await chatClient.GetResponseAsync(prompt, new ChatOptions
        {
            ModelId = llmOptions.Claude.ModelId,
            Temperature = 0.2f,
            MaxOutputTokens = 500
        });

        return response.Text;
    }
}