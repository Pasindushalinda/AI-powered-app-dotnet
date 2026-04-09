using Anthropic;
using Anthropic.Models.Messages;
using System.Collections.Concurrent;

namespace Server.Services;

public class ChatService(AnthropicClient client)
{
    private readonly ConcurrentDictionary<string, List<MessageParam>> _conversations = new();

    public async Task<string> ChatAsync(string conversationId, string prompt)
    {
        var history = _conversations.GetOrAdd(conversationId, _ => []);

        history.Add(new MessageParam { Role = Role.User, Content = prompt });

        var response = await client.Messages.Create(new MessageCreateParams
        {
            Model = Model.ClaudeHaiku4_5,
            MaxTokens = 100,
            Messages = [.. history]
        });

        var text = response.Content
            .Select(b => b.Value)
            .OfType<TextBlock>()
            .FirstOrDefault()?.Text ?? "";

        history.Add(new MessageParam { Role = Role.Assistant, Content = text });

        return text;
    }
}