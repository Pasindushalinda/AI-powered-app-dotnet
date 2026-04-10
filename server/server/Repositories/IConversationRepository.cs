using Microsoft.Extensions.AI;

namespace Server.Repositories;

public interface IConversationRepository
{
    List<ChatMessage> GetOrCreate(string conversationId);
    void Save(string conversationId, List<ChatMessage> messages);
}