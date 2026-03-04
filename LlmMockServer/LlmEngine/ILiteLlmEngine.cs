using LlmMockServer.Models;
using System.Threading;
using System.Threading.Tasks;

namespace LlmMockServer.LlmEngine;

public interface ILiteLlmEngine
{
    // Generation of full response (not a stream)
    Task<string> GenerateAsync(string prompt, int maxTokens, CancellationToken cancellationToken = default);

    // Async source of response chunks
    IAsyncEnumerable<LlmResponseChunk> GenerateStreamAsync(string prompt, int maxTokens, CancellationToken cancellationToken = default);
}
