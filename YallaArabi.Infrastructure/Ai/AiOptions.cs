namespace YallaArabi.Infrastructure.Ai;

// Bound from the "Ai" configuration section. Without an API key the fake provider is used.
public class AiOptions
{
    public const string ClaudeProvider = "claude";
    public const string FakeProvider = "fake";

    public string Provider { get; set; } = FakeProvider;
    public string? ApiKey { get; set; }
    public string Model { get; set; } = "claude-opus-5-5";

    // A dictionary lookup is a short, well-specified task, so low effort keeps it fast and cheap.
    public string Effort { get; set; } = "low";

    public int MaxTokens { get; set; } = 2000;

    public bool UseClaude =>
        string.Equals(Provider, ClaudeProvider, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(ApiKey);
}
