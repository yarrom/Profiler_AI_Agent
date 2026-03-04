using Microsoft.AspNetCore.Mvc;
using LlmMockServer.LlmEngine;
using LlmMockServer.Models;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace LlmMockServer.Controllers;

[ApiController]
[Route("v1")]
public class LlmController : ControllerBase
{
    private readonly ILiteLlmEngine _engine;

    public LlmController(ILiteLlmEngine engine)
    {
        _engine = engine;
    }

    // POST v1/generate -> returns full JSON response
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] LlmRequest request, CancellationToken cancellationToken)
    {
        var text = await _engine.GenerateAsync(request.Prompt, request.MaxTokens, cancellationToken);
        var response = new
        {
            id = Guid.NewGuid().ToString(),
            objectType = "text_completion",
            created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            model = "mock-litellm",
            choices = new[] { new { text = text, finish_reason = "stop" } }
        };
        return Ok(response);
    }

    // POST v1/stream -> streaming through SSE (Server-Sent Events), format: data: JSON\n\n
    [HttpPost("stream")]
    public async Task Stream([FromBody] LlmRequest request, CancellationToken cancellationToken)
    {
        Response.StatusCode = StatusCodes.Status200OK;
        Response.ContentType = "text/event-stream";
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("X-Accel-Buffering", "no"); // For nginx

        await using var responseStream = Response.BodyWriter.AsStream();
        var serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        await foreach (var chunk in _engine.GenerateStreamAsync(request.Prompt, request.MaxTokens, cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested) break;

            var payload = new {
                id = Guid.NewGuid().ToString(),
                objectType = "text_delta",
                created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                model = "mock-litellm",
                delta = new { content = chunk.Text },
                done = chunk.Done
            };

            var json = JsonSerializer.Serialize(payload, serializerOptions);
            var sse = $"data: {json}\n\n";
            var bytes = Encoding.UTF8.GetBytes(sse);
            await responseStream.WriteAsync(bytes, cancellationToken);
            await responseStream.FlushAsync(cancellationToken);

            if (chunk.Done) break;
        }
    }
}
