using Microsoft.Extensions.AI;
using System.Collections.Concurrent;

namespace Server.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ConcurrentDictionary<string, List<ChatMessage>> _conversations = new();

    public List<ChatMessage> GetOrCreate(string conversationId) =>
        _conversations.GetOrAdd(conversationId, _ => []);

    public void Save(string conversationId, List<ChatMessage> messages) =>
        _conversations[conversationId] = messages;
}