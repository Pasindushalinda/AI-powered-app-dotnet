using FluentValidation;
using Microsoft.Extensions.AI;
using Server.Models;
using Server.Services;
using Server.Validators;

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

builder.Services.AddSingleton<IValidator<ChatRequest>, ChatRequestValidator>();
builder.Services.AddSingleton<ChatService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/api/chat", async (ChatRequest request, IValidator<ChatRequest> validator, ChatService chatService) =>
{
    var result = await validator.ValidateAsync(request);
    if (!result.IsValid)
    {
        var errors = result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        return Results.BadRequest(errors);
    }

    var message = await chatService.ChatAsync(
        request.ConversationId,
        request.Prompt.Trim(),
        request.Provider
    );
    return Results.Json(new { message });
});

app.MapGet("/api/provider", (LlmOptions opts) =>
    Results.Json(new { activeProvider = opts.Provider }));

app.Run();