using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using server.Data;
using server.Endpoints;
using server.Models;
using server.Repositories;
using server.Services;
using server.Validators;

var builder = WebApplication.CreateBuilder(args);

var llmOptions = builder.Configuration
    .GetSection("AI")
    .Get<LlmOptions>() ?? new LlmOptions();

builder.Services.AddSingleton(llmOptions);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var inner = LlmClientFactory.Create(llmOptions.Provider, llmOptions);

    return new ChatClientBuilder(inner)
        .UseLogging(loggerFactory)
        .Build(sp);
});

builder.Services.AddSingleton<IValidator<ChatRequest>, ChatRequestValidator>();
builder.Services.AddSingleton<IConversationRepository, ConversationRepository>();
builder.Services.AddSingleton<ChatService>();

var app = builder.Build();

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;

    var (status, message) = ex switch
    {
        ArgumentException e => (400, e.Message),
        _                   => (500, "Failed to generate a response.")
    };

    ctx.Response.StatusCode = status;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { error = message });
}));

app.MapGet("/", () => "Hello World!");

app.MapChatEndpoints();

app.Run();