using Anthropic.SDK;
using Azure.AI.OpenAI;
using GeminiDotnet;
using GeminiDotnet.Extensions.AI;
using Microsoft.Extensions.AI;
using Server.Models;
using System.ClientModel;

namespace Server.Services;

public static class LlmClientFactory
{
    public static IChatClient Create(string provider, LlmOptions options)
    {
        return provider.ToLower() switch
        {
            "openai" => new OpenAI.Chat.ChatClient(
                            options.OpenAI.ModelId,
                            options.OpenAI.ApiKey)
                            .AsIChatClient(),

            "azure" => new AzureOpenAIClient(
                            new Uri(options.Azure.Endpoint),
                            new ApiKeyCredential(options.Azure.ApiKey))
                            .GetChatClient(options.Azure.DeploymentName)
                            .AsIChatClient(),

            "gemini" => new GeminiChatClient(
                            new GeminiClientOptions
                            {
                                ApiKey = options.Gemini.ApiKey,
                                ModelId = options.Gemini.ModelId,
                                ApiVersion = GeminiApiVersions.V1Beta
                            }),

            "claude" => new AnthropicClient(
                            new APIAuthentication(options.Claude.ApiKey))
                            .Messages,

            _ => throw new ArgumentException($"Unknown provider: {provider}")
        };
    }
}