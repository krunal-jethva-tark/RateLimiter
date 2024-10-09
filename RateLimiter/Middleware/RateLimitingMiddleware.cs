using Microsoft.AspNetCore.Http;

namespace RateLimiter.Middleware;

/// <summary>
/// Middleware that applies rate limiting strategies to incoming HTTP requests.
/// </summary>
public class RateLimitingMiddleware(RequestDelegate next, RateLimiterPolicyRegistry policyRegistry)
{
    /// <summary>
    /// Invokes the middleware to process the HTTP request.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        foreach (var policy in policyRegistry.Policies)
        {
            var strategy = policy.Value(context);

            if (!await strategy.IsRequestPermittedAsync(context))  
            {
                context.Response.StatusCode = strategy.Options.RejectionStatusCode;
                await context.Response.WriteAsync(strategy.Options.RejectionMessage);
                return;
            }
        }

        await next(context);
    }
}