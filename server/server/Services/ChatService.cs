using Microsoft.Extensions.AI;
using server.Models;
using server.Repositories;

namespace server.Services;

public class ChatService(
    IChatClient defaultClient,
    LlmOptions llmOptions,
    IConversationRepository conversationRepository,
    ILoggerFactory loggerFactory,
    IWebHostEnvironment environment)
{
    private readonly string _systemPrompt = BuildSystemPrompt(environment);

    private static string BuildSystemPrompt(IWebHostEnvironment env)
    {
        var promptsPath = Path.Combine(env.ContentRootPath, "Prompts");
        var template = File.ReadAllText(Path.Combine(promptsPath, "chatbot.txt"));
        var parkInfo  = File.ReadAllText(Path.Combine(promptsPath, "WonderWorld.md"));
        return template.Replace("{{parkInfo}}", parkInfo);
    }

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

        if (history.Count == 0)
            history.Add(new ChatMessage(ChatRole.System, _systemPrompt));

        history.Add(new ChatMessage(ChatRole.User, prompt));

        var response = await client.GetResponseAsync(history, new ChatOptions
        {
            ModelId = ResolveModelId(activeProvider),
            MaxOutputTokens = 200,
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