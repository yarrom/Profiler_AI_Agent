namespace LlmMockServer.Models;

public class LlmRequest
{
    // Prompt text
    public string Prompt { get; set; } = string.Empty;

    // Max response length
    public int MaxTokens { get; set; } = 128;

    // Flag of stream usage (SSE)
    public bool Stream { get; set; } = false;
}
