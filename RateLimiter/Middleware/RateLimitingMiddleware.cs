using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using RateLimiter.Attributes;
using RateLimiter.Models;
using RateLimiter.Strategies;

namespace RateLimiter.Middleware;

/// <summary>
/// Middleware that applies rate limiting strategies to incoming HTTP requests.
/// This middleware inspects the request and enforces rate limiting based on registered policies.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimiterPolicyRegistry _policyRegistry;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitingMetrics _metrics;
    
    public RateLimitingMiddleware(RequestDelegate next, RateLimiterPolicyRegistry policyRegistry, ILogger<RateLimitingMiddleware> logger, RateLimitingMetrics metrics)
    {
        _next = next;
        _policyRegistry = policyRegistry;
        _logger = logger;
        _metrics = metrics;
    }
    
    /// <summary>
    /// Invokes the middleware to process the HTTP request.
    /// Checks for applicable rate limiting policies and processes the request accordingly.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (IsRateLimitingEnabled(context) && await TryApplyingRateLimitingPolicies(context))
        {
            return;
        }
        await _next(context);
    }

    private async Task<bool> TryApplyingRateLimitingPolicies(HttpContext context)
    {
        var policyName = GetRateLimitingPolicyName(context);
        if (policyName != null)
        {
            await ProcessPolicy(context, policyName);
            return true;
        }
        
        // Try to apply global policies if no specific policy was found
        var defaultPolicy = _policyRegistry.DefaultPolicyName;
        if (defaultPolicy != null)
        {
            await ProcessPolicy(context, defaultPolicy);
            return true;
        }
        
        // return false if no policy was applied
        return false;
    }
    
    private static bool IsRateLimitingEnabled(HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        return endpoint?.Metadata.GetMetadata<DisableRateLimiting>() == null;
    }
    
    private static string? GetRateLimitingPolicyName(HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var rateLimitingAttr = endpoint?.Metadata.GetMetadata<EnableRateLimiting>();
        return rateLimitingAttr?.PolicyName;
    }

    private async Task ProcessPolicy(HttpContext context, string policyName)
    {
        var policyFactory = _policyRegistry.GetPolicy(policyName);
        
        if (policyFactory == null) return;

        var strategy = policyFactory(context);
        await ApplyRateLimitingStrategy(context, policyName, strategy);
    }
    
    private async Task ApplyRateLimitingStrategy(HttpContext context, string policyName, RateLimiterStrategyBase<RateLimiterStrategyOptions> strategy)
    {
        var key = strategy.Options.KeyGenerator(context);
        var startTimestamp = Stopwatch.GetTimestamp();
        _metrics.LeaseStart(policyName);
        try
        {
            if (await strategy.IsRequestPermittedAsync(key, DateTime.UtcNow))
            {
                await _next(context);
            }
            else
            {
                _metrics.LeaseFailed(policyName, RequestRejectionReason.GlobalLimiter);
                await RejectRequest(context, strategy.Options.RejectionMessage, strategy.Options.RejectionStatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing request with strategy: {strategy.GetType().Name}");
            _metrics.LeaseFailed(policyName, RequestRejectionReason.RequestCanceled);
            await RejectRequest(context, "Internal server error.", StatusCodes.Status500InternalServerError);
        }
        finally
        {
            var endTimeStamp = Stopwatch.GetTimestamp();
            var duration = Stopwatch.GetElapsedTime(startTimestamp, endTimeStamp);
            _metrics.LeaseEnd(policyName, duration);
        }
    }

    private static async Task RejectRequest(HttpContext context, string message = "Rate limit exceeded. Please try again later.", int statusCode = StatusCodes.Status429TooManyRequests)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
}