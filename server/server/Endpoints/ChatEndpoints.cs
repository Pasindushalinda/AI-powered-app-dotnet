using server.Filters;
using server.Models;
using server.Services;

namespace server.Endpoints;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        app.MapPost("/api/chat", async (ChatRequest request, ChatService chatService) =>
        {
            var message = await chatService.ChatAsync(
                request.ConversationId,
                request.Prompt.Trim(),
                request.Provider
            );
            return Results.Json(new { message });
        }).AddEndpointFilter<ValidationFilter<ChatRequest>>();

        app.MapGet("/api/provider", (LlmOptions opts) =>
            Results.Json(new { activeProvider = opts.Provider }));
    }
}