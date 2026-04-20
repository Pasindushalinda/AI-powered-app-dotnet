namespace server.Models;

public record ChatRequest(
    string Prompt,
    string ConversationId,
    string? Provider = null
);