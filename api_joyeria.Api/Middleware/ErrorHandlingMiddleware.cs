using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace api_joyeria.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException kex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new { error = kex.Message });
            _logger.LogWarning(kex, "Recurso no encontrado");
            await context.Response.WriteAsync(payload);
        }
        catch (InvalidOperationException iex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new { error = iex.Message });
            _logger.LogWarning(iex, "Invalid operation");
            await context.Response.WriteAsync(payload);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new { error = "Internal server error" });
            _logger.LogError(ex, "Unhandled exception");
            await context.Response.WriteAsync(payload);
        }
    }
}