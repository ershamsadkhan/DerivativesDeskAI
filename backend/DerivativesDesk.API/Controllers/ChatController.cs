using DerivativesDesk.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DerivativesDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatService chatService, ILogger<ChatController> logger) : ControllerBase
{
    /// <summary>Stream a chat response via Server-Sent Events.</summary>
    [HttpPost("stream")]
    public async Task StreamAsync([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            Response.StatusCode = 400;
            return;
        }

        // Disable Kestrel/middleware buffering so each WriteAsync goes out immediately
        var bufferingFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpResponseBodyFeature>();
        bufferingFeature?.DisableBuffering();

        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";
        Response.Headers["X-Accel-Buffering"] = "no";

        // Flush headers immediately so the browser gets a 200 and doesn't stay in "pending"
        await Response.StartAsync(cancellationToken);

        logger.LogInformation("Chat stream started for session {SessionId}", request.SessionId);

        try
        {
            await foreach (var token in chatService.StreamAsync(request.SessionId, request.Message, cancellationToken))
            {
                var data = System.Text.Json.JsonSerializer.Serialize(new { token });
                await Response.WriteAsync($"data: {data}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            await Response.WriteAsync("data: [DONE]\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Chat stream cancelled for session {SessionId}", request.SessionId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Chat stream error for session {SessionId}", request.SessionId);
            var errData = System.Text.Json.JsonSerializer.Serialize(new { error = ex.Message });
            await Response.WriteAsync($"data: {errData}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    /// <summary>Health check — returns 200 if the API is running.</summary>
    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new { status = "ok", utc = DateTime.UtcNow });

    /// <summary>Get chat history for a session.</summary>
    [HttpGet("history/{sessionId}")]
    public async Task<IActionResult> GetHistoryAsync(string sessionId)
    {
        var history = await chatService.GetHistoryAsync(sessionId);
        var messages = history.Select(h => new { role = h.Role, content = h.Content });
        return Ok(new { sessionId, messages });
    }
}

public record ChatRequest(string SessionId, string Message);
