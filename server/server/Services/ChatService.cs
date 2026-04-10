using Microsoft.Extensions.AI;
using Server.Models;
using Server.Repositories;

namespace Server.Services;

public class ChatService(
    IChatClient defaultClient,
    LlmOptions llmOptions,
    IConversationRepository conversationRepository,
    ILoggerFactory loggerFactory)
{
    public async Task<string> ChatAsync(
        string conversationId,
        string prompt,
        string? providerOverride = null)
    {
        var activeProvider = providerOverride ?? llmOptions.Provider;
        var client = providerOverride is not null
            ? BuildClient(LlmClientFactory.Create(providerOverride, llmOptions))
            : defaultClient;

        var history = conversationRepository.GetOrCreate(conversationId);

        history.Add(new ChatMessage(ChatRole.User, prompt));

        var response = await client.GetResponseAsync(history, new ChatOptions
        {
            ModelId = ResolveModelId(activeProvider),
            MaxOutputTokens = 1000,
            Temperature = 0.7f
        });

        var text = response.Text ?? "";
        history.AddRange(response.Messages);
        conversationRepository.Save(conversationId, history);

        return text;
    }

    private string ResolveModelId(string provider) => provider.ToLower() switch
    {
        "openai" => llmOptions.OpenAI.ModelId,
        "azure"  => llmOptions.Azure.DeploymentName,
        "gemini" => llmOptions.Gemini.ModelId,
        "claude" => llmOptions.Claude.ModelId,
        _        => throw new ArgumentException($"Unknown provider: {provider}")
    };

    private IChatClient BuildClient(IChatClient inner) =>
        new ChatClientBuilder(inner)
            .UseLogging(loggerFactory)
            .Build();
}