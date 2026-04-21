using server.Domain;
using server.Llm;
using server.Repositories;

namespace server.Services;

public class ReviewService(IReviewRepository reviewRepository, LlmClient llmClient, IWebHostEnvironment environment)
{
    private readonly string _promptTemplate = File.ReadAllText(
        Path.Combine(environment.ContentRootPath, "Prompts", "summarize-reviews.txt"));

    public Task<List<Review>> GetReviewsAsync(int productId) =>
        reviewRepository.GetReviewsAsync(productId);

    public async Task<string> SummarizeReviewsAsync(int productId)
    {
        var reviews = await reviewRepository.GetReviewsAsync(productId, limit: 10);
        var joinedReviews = string.Join("\n\n", reviews.Select(r => r.Content));

        var prompt = _promptTemplate.Replace("{{reviews}}", joinedReviews);

        var result = await llmClient.GenerateTextAsync(new GenerateTextOptions
        {
            Prompt = prompt,
            Temperature = 0.2f,
            MaxTokens = 500
        });

        return result.Text;
    }
}
