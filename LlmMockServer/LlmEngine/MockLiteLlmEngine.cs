using LlmMockServer.Models;
using System.Text;
using System.Threading.Tasks;

namespace LlmMockServer.LlmEngine;

public class MockLiteLlmEngine : ILiteLlmEngine
{
    // Simple imitation: breaks response to tokens by spaces and returns with delay
    public async Task<string> GenerateAsync(string prompt, int maxTokens, CancellationToken cancellationToken = default)
    {
        // Build "response"
        var sb = new StringBuilder();
        sb.Append("Mock response for prompt: ");
        sb.Append(prompt);
        // Limit length
        var result = sb.ToString();
        if (result.Length > maxTokens)
            result = result.Substring(0, maxTokens);
        await Task.Delay(50, cancellationToken);
        return result;
    }

    public async IAsyncEnumerable<LlmResponseChunk> GenerateStreamAsync(string prompt, int maxTokens, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Generate text subdivided to words 
        string baseText = "Mock streaming response for prompt: " + prompt;
        var words = baseText.Split(' ');
        int usedTokens = 0;
        foreach (var w in words)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            // Tokens limit
            usedTokens += w.Length;
            if (usedTokens > maxTokens) break;

            // Return chunk
            yield return new LlmResponseChunk { Text = w + " ", Done = false };
            await Task.Delay(120, cancellationToken);
        }

        // Final mark
        yield return new LlmResponseChunk { Text = string.Empty, Done = true };
    }
}
