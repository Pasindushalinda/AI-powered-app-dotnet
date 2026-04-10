using Microsoft.Extensions.AI;
using System.Collections.Concurrent;

namespace Server.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _conversations = new();

    public List<ChatMessage> GetOrCreate(string conversationId) =>
        _conversations.GetOrAdd(conversationId, _ =>
        [
            new ChatMessage(ChatRole.System, "You are a helpful assistant.")
        ]);

    public void Save(string conversationId, List<ChatMessage> messages) =>
        _conversations[conversationId] = messages;
}