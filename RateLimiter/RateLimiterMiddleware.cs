using Microsoft.AspNetCore.Http;
using RateLimiter.Interfaces;

namespace RateLimiter;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiterStrategy _rateLimiter;

    public RateLimiterMiddleware(RequestDelegate next, IRateLimiterStrategy rateLimiter)
    {
        _next = next;
        _rateLimiter = rateLimiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
    }
}