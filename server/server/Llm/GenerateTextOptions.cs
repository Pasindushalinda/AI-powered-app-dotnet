namespace server.Llm;

public record GenerateTextOptions
{
    public required string Prompt { get; init; }
    public string? ModelId { get; init; }
    public string? Instructions { get; init; }
    public float Temperature { get; init; } = 0.2f;
    public int MaxTokens { get; init; } = 300;
}
