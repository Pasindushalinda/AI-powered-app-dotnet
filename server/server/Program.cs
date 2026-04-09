using Anthropic;
using Server.Models;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ => new AnthropicClient
{
    ApiKey = builder.Configuration["ANTHROPIC_API_KEY"]
          ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
});
builder.Services.AddSingleton<ChatService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/hello", () => Results.Json(new { message = "Hello World!" }));

app.MapPost("/api/chat", async (ChatRequest request, ChatService chatService) =>
{
    var message = await chatService.ChatAsync(request.ConversationId, request.Prompt);
    return Results.Json(new { message });
});

app.Run();