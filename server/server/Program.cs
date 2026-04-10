using Microsoft.Extensions.AI;
using Server.Models;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

var llmOptions = builder.Configuration
    .GetSection("AI")
    .Get<LlmOptions>() ?? new LlmOptions();

builder.Services.AddSingleton(llmOptions);

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var inner = LlmClientFactory.Create(llmOptions.Provider, llmOptions);

    return new ChatClientBuilder(inner)
        .UseLogging(loggerFactory)
        .Build(sp);
});

builder.Services.AddSingleton<ChatService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/api/chat", async (ChatRequest request, ChatService chatService) =>
{
    var message = await chatService.ChatAsync(
        request.ConversationId,
        request.Prompt,
        request.Provider
    );
    return Results.Json(new { message });
});

app.MapGet("/api/provider", (LlmOptions opts) =>
    Results.Json(new { activeProvider = opts.Provider }));

app.Run();