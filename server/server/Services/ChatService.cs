using Microsoft.Extensions.AI;
using server.Llm;
using server.Repositories;

namespace server.Services;

public class ChatService(
    LlmClient llmClient,
    IConversationRepository conversationRepository,
    IWebHostEnvironment environment)
{
    private readonly string _instructions = BuildSystemPrompt(environment);

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
        var history = conversationRepository.GetOrCreate(conversationId);
        var fullPrompt = BuildPrompt(history, prompt);

        var result = await llmClient.GenerateTextAsync(new GenerateTextOptions
        {
            Prompt = fullPrompt,
            Instructions = _instructions,
            Temperature = 0.7f,
            MaxTokens = 200
        });

        history.Add(new ChatMessage(ChatRole.User, prompt));
        history.Add(new ChatMessage(ChatRole.Assistant, result.Text));
        conversationRepository.Save(conversationId, history);

        return result.Text;
    }

    private static string BuildPrompt(IList<ChatMessage> history, string newPrompt)
    {
        if (history.Count == 0)
            return newPrompt;

        var lines = history
            .Where(m => m.Role != ChatRole.System)
            .Select(m => $"{m.Role}: {m.Text}");

        return string.Join("\n", lines) + $"\nuser: {newPrompt}";
    }
}
