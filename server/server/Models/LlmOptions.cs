namespace Server.Models;

public class LlmOptions
{
    public string Provider { get; set; } = "claude";
    public OpenAIOptions OpenAI { get; set; } = new();
    public AzureOptions Azure { get; set; } = new();
    public GeminiOptions Gemini { get; set; } = new();
    public ClaudeOptions Claude { get; set; } = new();
}

public class OpenAIOptions
{
    public string ApiKey { get; set; } = "";
    public string ModelId { get; set; } = "gpt-4.1-mini";
}

public class AzureOptions
{
    public string Endpoint { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string DeploymentName { get; set; } = "";
}

public class GeminiOptions
{
    public string ApiKey { get; set; } = "";
    public string ModelId { get; set; } = "gemini-2.0-flash";
}

public class ClaudeOptions
{
    public string ApiKey { get; set; } = "";
    public string ModelId { get; set; } = "claude-haiku-4-5";
}