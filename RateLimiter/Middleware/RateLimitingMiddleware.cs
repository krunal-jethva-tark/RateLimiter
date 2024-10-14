using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using RateLimiter.Filters;

namespace RateLimiter.Middleware;

/// <summary>
/// Middleware that applies rate limiting strategies to incoming HTTP requests.
/// This middleware inspects the request and enforces rate limiting based on registered policies.
/// </summary>
public class RateLimitingMiddleware(RequestDelegate next, RateLimiterPolicyRegistry policyRegistry)
{
    /// <summary>
    /// Invokes the middleware to process the HTTP request.
    /// It checks for applicable rate limiting policies and processes the request accordingly.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var policyName = GetRateLimitingPolicyName(context);
        if (policyName != null)
        {
            if (await TryProcessRequestWithPolicy(context, policyName))
            {
                return;
            }
        }

        if (await TryProcessRequestWithGlobalPolicies(context))
        {
            return;
        }

        await RejectRequest(context, "Rate limit exceeded for the requested resource.");
    }
    
    private static string? GetRateLimitingPolicyName(HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var rateLimitingAttr = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();
        return rateLimitingAttr?.PolicyName;
    }
    
    private async Task<bool> TryProcessRequestWithPolicy(HttpContext context, string policyName)
    {
        var policyFactory = policyRegistry.GetPolicy(policyName);
        if (policyFactory == null) return false;

        var (strategy, isGlobal) = policyFactory(context);
        var key = strategy.Options.KeyGenerator(context);

        if (isGlobal && await strategy.IsRequestPermittedAsync(key, DateTime.UtcNow))
        {
            await next(context);
            return true;
        }

        await RejectRequest(context, strategy.Options.RejectionMessage, strategy.Options.RejectionStatusCode);
        return false;
    }
    
    private async Task<bool> TryProcessRequestWithGlobalPolicies(HttpContext context)
    {
        foreach (var policy in policyRegistry.Policies)
        {
            var (strategy, isGlobal) = policy.Value(context);
            var key = strategy.Options.KeyGenerator(context);

            if (isGlobal && await strategy.IsRequestPermittedAsync(key, DateTime.UtcNow))
            {
                await next(context);
                return true;
            }
        }
        return false;
    }
    
    private static async Task RejectRequest(HttpContext context, string message, int statusCode = StatusCodes.Status429TooManyRequests)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
    
}