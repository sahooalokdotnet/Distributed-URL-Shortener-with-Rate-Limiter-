using StackExchange.Redis;

namespace UrlShortener.UrlShortener.API.Middleware;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDatabase _redis;
    private const int MaxRequests = 100;
    private const int WindowSeconds = 60;

    public RateLimiterMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
    {
        _next = next;
        _redis = redis.GetDatabase();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rate:{ip}";

        var count = await _redis.StringIncrementAsync(key);
        if (count == 1)
            await _redis.KeyExpireAsync(key, TimeSpan.FromSeconds(WindowSeconds));

        if (count > MaxRequests)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Rate limit exceeded. Try again in 60 seconds.");
            return;
        }

        context.Response.Headers["X-RateLimit-Remaining"] =
            (MaxRequests - count).ToString();

        await _next(context);
    }
}