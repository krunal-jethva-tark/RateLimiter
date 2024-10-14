using System.Collections.Concurrent;
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
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockStore = new();
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
            if (!await TryProcessRequestWithPolicy(context, policyName))
            {
                return;
            }
        }

        if (!await TryProcessRequestWithGlobalPolicies(context))
        {
            return;
        }

        await next(context);
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

        // Use a lock object for the specific key
        var lockObject = _lockStore.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await lockObject.WaitAsync();
        try
        {
            if (isGlobal && await strategy.IsRequestPermittedAsync(key, DateTime.UtcNow))
            {
                return true;
            }

            await RejectRequest(context, strategy.Options.RejectionMessage, strategy.Options.RejectionStatusCode);
            return false;
        }
        catch (Exception ex)
        {
            // Log the exception (implement logging as per your application)
            Console.WriteLine($"Error processing request with policy {policyName}: {ex.Message}");
            await RejectRequest(context, "Internal server error.", StatusCodes.Status500InternalServerError);
            return false;
        }
        finally
        {
            lockObject.Release();
        }
    }
    
    private async Task<bool> TryProcessRequestWithGlobalPolicies(HttpContext context)
    {
        foreach (var policy in policyRegistry.Policies)
        {
            var (strategy, isGlobal) = policy.Value(context);
            var key = strategy.Options.KeyGenerator(context);
            // Use a lock object for the specific key
            var lockObject = _lockStore.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await lockObject.WaitAsync();
            try
            {
                if (isGlobal && await strategy.IsRequestPermittedAsync(key, DateTime.UtcNow))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging as per your application)
                Console.WriteLine($"Error processing request with policy {policy.Key}: {ex.Message}");
                await RejectRequest(context, "Internal server error.", StatusCodes.Status500InternalServerError);
                return false;
            }
            finally
            {
                lockObject.Release();
            }
        }
        await RejectRequest(context);
        return false;
    }
    
    private static async Task RejectRequest(HttpContext context, string message = "Rate limit exceeded. Please try again later.", int statusCode = StatusCodes.Status429TooManyRequests)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
    
}