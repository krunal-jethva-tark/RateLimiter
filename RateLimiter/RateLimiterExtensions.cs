using RateLimiter.Models;
using RateLimiter.Stores;
using RateLimiter.Strategies;

namespace RateLimiter;

/// <summary>
/// Provides extension methods for registering different rate limiting policies 
/// within the application.
/// </summary>
public static class RateLimiterExtensions
{
    /// <summary>
    /// Registers a Fixed Window rate limiting policy in the provided policy registry.
    /// The Fixed Window strategy limits the number of requests during a defined time window.
    /// The window resets after each interval, and the request count is reset to zero.
    /// </summary>
    /// <param name="registry">
    /// The <see cref="RateLimiterPolicyRegistry"/> where the rate limiting policy will be registered.
    /// </param>
    /// <param name="policyName">
    /// A string that uniquely identifies the rate limiting policy.
    /// </param>
    /// <param name="configure">
    /// A delegate used to configure the options for the Fixed Window policy, including window size and request limits.
    /// For example, setting a window of 1 minute and a request limit of 100 allows up to 100 requests per minute.
    /// </param>
    /// <example>
    /// Example usage:
    /// <code>
    /// registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
    /// {
    ///     options.WindowSize = TimeSpan.FromMinutes(1);
    ///     options.RequestLimit = 100;
    /// });
    /// </code>
    /// </example>
    /// <returns>
    /// The <see cref="RateLimiterPolicyRegistry"/> instance for further chaining.
    /// </returns>
    public static RateLimiterPolicyRegistry AddFixedWindowPolicy(this RateLimiterPolicyRegistry registry, string policyName, Action<FixedWindowOptions> configure)
    {
        var options = new FixedWindowOptions();
        configure(options);
        registry.RegisterPolicy(policyName, context =>
        {
            var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
            return new FixedWindowRateStrategy(counterStore, options);
        });
        return registry;
    }

    /// <summary>
    /// Registers a Token Bucket rate limiting policy in the provided policy registry.
    /// The Token Bucket strategy controls request rates by allowing requests until the token bucket is empty.
    /// Tokens are refilled at a regular rate, allowing for burst traffic handling.
    /// </summary>
    /// <param name="registry">
    /// The <see cref="RateLimiterPolicyRegistry"/> where the rate limiting policy will be registered.
    /// </param>
    /// <param name="policyName">
    /// A string that uniquely identifies the rate limiting policy.
    /// </param>
    /// <param name="configure">
    /// A delegate used to configure the options for the Token Bucket policy, such as token limit and refill rate.
    /// For instance, setting a token limit of 200 and a refill rate of 5 tokens per second
    /// allows a burst of up to 200 requests, refilling at 5 requests per second.
    /// </param>
    /// <example>
    /// Example usage:
    /// <code>
    /// registry.AddTokenBucketPolicy("TokenBucketPolicy", options =>
    /// {
    ///     options.TokenLimit = 200;
    ///     options.RefillInterval = TimeSpan.FromSeconds(1);
    ///     options.RefillAmount = 5;
    /// });
    /// </code>
    /// </example>
    /// <returns>
    /// The <see cref="RateLimiterPolicyRegistry"/> instance for further chaining.
    /// </returns>
    public static RateLimiterPolicyRegistry AddTokenBucketPolicy(this RateLimiterPolicyRegistry registry, string policyName, Action<TokenBucketOptions> configure)
    {
        var options = new TokenBucketOptions();
        configure(options);

        registry.RegisterPolicy(policyName, context =>
        {
            var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
            return new TokenBucketRateStrategy(counterStore, options);
        });
        return registry;
    }
    
    /// <summary>
    /// Marks the last registered rate limiting policy as the default policy.
    /// This method sets the default policy for the rate limiter, which will be used if no specific policy is specified.
    /// </summary>
    /// <param name="registry">
    /// The <see cref="RateLimiterPolicyRegistry"/> where the default policy will be set.
    /// </param>
    /// <returns>
    /// The <see cref="RateLimiterPolicyRegistry"/> instance for further chaining.
    /// </returns>
    public static RateLimiterPolicyRegistry MarkAsDefault(this RateLimiterPolicyRegistry registry)
    {
        registry.SetDefaultPolicy(); // Set the last registered policy as default
        return registry; // Return registry for further chaining
    }
}
