using Microsoft.Extensions.AI;
using server.Models;

namespace server.Llm;

public class LlmClient(IChatClient defaultChatClient, LlmOptions llmOptions)
{
    public async Task<GenerateTextResult> GenerateTextAsync(GenerateTextOptions options)
    {
        var modelId = options.ModelId ?? ResolveDefaultModelId();

        var messages = new List<ChatMessage>();

        if (options.Instructions is not null)
            messages.Add(new ChatMessage(ChatRole.System, options.Instructions));

        messages.Add(new ChatMessage(ChatRole.User, options.Prompt));

        var response = await defaultChatClient.GetResponseAsync(messages, new ChatOptions
        {
            ModelId = modelId,
            Temperature = options.Temperature,
            MaxOutputTokens = options.MaxTokens
        });

        return new GenerateTextResult(response.Text);
    }

    private string ResolveDefaultModelId() => llmOptions.Provider.ToLower() switch
    {
        "openai" => llmOptions.OpenAI.ModelId,
        "azure"  => llmOptions.Azure.DeploymentName,
        "gemini" => llmOptions.Gemini.ModelId,
        "claude" => llmOptions.Claude.ModelId,
        _        => throw new ArgumentException($"Unknown provider: {llmOptions.Provider}")
    };
}
