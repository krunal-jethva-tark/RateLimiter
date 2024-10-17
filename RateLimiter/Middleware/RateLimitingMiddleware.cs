using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Prometheus;
using RateLimiter.Filters;
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
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _lockStore = new();

    // Define Prometheus metrics
    private static readonly Counter TotalRequests = Metrics.CreateCounter("total_requests", "Total number of HTTP requests.");
    private static readonly Counter AcceptedRequests = Metrics.CreateCounter("accepted_requests", "Number of accepted HTTP requests (status 200).");
    private static readonly Counter RejectedRequests = Metrics.CreateCounter("rejected_requests", "Number of rejected HTTP requests (status 429).");
    
    public RateLimitingMiddleware(RequestDelegate next, RateLimiterPolicyRegistry policyRegistry, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _policyRegistry = policyRegistry;
        _logger = logger;
    }
    
    /// <summary>
    /// Invokes the middleware to process the HTTP request.
    /// It checks for applicable rate limiting policies and processes the request accordingly.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        TotalRequests.Inc();

        var policyName = GetRateLimitingPolicyName(context);
        if (policyName != null && !await ProcessPolicy(context, policyName))
        {
            return;
        }

        if (!await ProcessGlobalPolicies(context))
        {
            return;
        }

        // Request is accepted
        AcceptedRequests.Inc();
        await _next(context);
    }
    
    private static string? GetRateLimitingPolicyName(HttpContext context)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var rateLimitingAttr = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();
        return rateLimitingAttr?.PolicyName;
    }

    private async Task<bool> ProcessPolicy(HttpContext context, string policyName)
    {
        var policyFactory = _policyRegistry.GetPolicy(policyName);
        if (policyFactory == null) return false;

        var (strategy, _) = policyFactory(context);
        return await ExecuteRateLimitingStrategy(context, strategy);
    }

    private async Task<bool> ProcessGlobalPolicies(HttpContext context)
    {
        foreach (var policy in _policyRegistry.Policies)
        {
            var (strategy, isGlobal) = policy.Value(context);
            if (isGlobal && await ExecuteRateLimitingStrategy(context, strategy))
            {
                return true;
            }
        }

        await RejectRequest(context);
        return false;
    }
    
    private async Task<bool> ExecuteRateLimitingStrategy(HttpContext context, RateLimiterStrategyBase<RateLimiterStrategyOptions> strategy)
    {
        var key = strategy.Options.KeyGenerator(context);
        var lockObject = _lockStore.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        await lockObject.WaitAsync();
        try
        {
            if (await strategy.IsRequestPermittedAsync(key, DateTime.UtcNow))
            {
                return true;
            }

            await RejectRequest(context, strategy.Options.RejectionMessage, strategy.Options.RejectionStatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing request with strategy: {strategy.GetType().Name}");
            await RejectRequest(context, "Internal server error.", StatusCodes.Status500InternalServerError);
            return false;
        }
        finally
        {
            lockObject.Release();
        }
    }

    private static async Task RejectRequest(HttpContext context, string message = "Rate limit exceeded. Please try again later.", int statusCode = StatusCodes.Status429TooManyRequests)
    {
        RejectedRequests.Inc();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(message);
    }
}