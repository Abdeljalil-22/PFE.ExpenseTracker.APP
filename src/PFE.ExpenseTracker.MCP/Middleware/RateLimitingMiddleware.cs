using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PFE.ExpenseTracker.MCP.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _requests = new();
    private const int LIMIT = 10; // 10 requests
    private static readonly TimeSpan WINDOW = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var now = DateTime.UtcNow;
        var (count, windowStart) = _requests.GetOrAdd(key, _ => (0, now));

        if (now - windowStart > WINDOW)
        {
            _requests[key] = (1, now);
        }
        else
        {
            if (count >= LIMIT)
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                return;
            }
            _requests[key] = (count + 1, windowStart);
        }

        await _next(context);
    }
}
