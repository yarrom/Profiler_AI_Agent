namespace LlmMockServer.Models;

public class LlmResponseChunk
{
    // Text response chunk
    public string Text { get; set; } = string.Empty;

    // Flag, signalling finishing
    public bool Done { get; set; } = false;
}
