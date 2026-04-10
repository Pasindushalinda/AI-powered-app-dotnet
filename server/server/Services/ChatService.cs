using Microsoft.Extensions.AI;
using Server.Models;
using System.Collections.Concurrent;

namespace Server.Services;

public class ChatService(
    IChatClient defaultClient,
    LlmOptions llmOptions,
    ILoggerFactory loggerFactory)
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _conversations = new();

    public async Task<string> ChatAsync(
        string conversationId,
        string prompt,
        string? providerOverride = null)
    {
        var activeProvider = providerOverride ?? llmOptions.Provider;
        var client = providerOverride is not null
            ? BuildClient(LlmClientFactory.Create(providerOverride, llmOptions))
            : defaultClient;

        var history = _conversations.GetOrAdd(conversationId, _ =>
        [
            new ChatMessage(ChatRole.System, "You are a helpful assistant.")
        ]);

        history.Add(new ChatMessage(ChatRole.User, prompt));

        var response = await client.GetResponseAsync(history, new ChatOptions
        {
            ModelId = ResolveModelId(activeProvider),
            MaxOutputTokens = 1000,
            Temperature = 0.7f
        });

        var text = response.Text ?? "";
        history.AddRange(response.Messages);

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