using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Strategies;

namespace RateLimiter;

/// <summary>
/// The <see cref="RateLimiterPolicyRegistry"/> class is responsible for managing and registering
/// rate limiting policies that control how requests are processed based on defined strategies.
/// This class allows for the registration, retrieval, and management of different rate limiting policies,
/// enabling fine-grained control over how rate limits are applied to various parts of an application.
/// </summary>
public class RateLimiterPolicyRegistry
{
    /// <summary>
    /// Gets or sets the default rate limiting policy. This policy will be applied if no specific policy is specified.
    /// The default policy is used as a fallback mechanism to ensure that a rate limiting strategy is always in place.
    /// </summary>
    public string? DefaultPolicyName { get; private set; }

    private string? _lastRegisteredPolicy; // Track the last registered policy

    /// <summary>
    /// A dictionary that holds all the registered rate limiting policies.
    /// Each entry in the dictionary maps a policy name to its corresponding strategy factory.
    /// </summary>
    /// <remarks>
    /// The <see cref="Policies"/> dictionary ensures that each policy is identified by a unique name.
    /// The strategy factory is used to create instances of rate limiting strategies based on the current HTTP request context.
    /// </remarks>
    private Dictionary<string, Func<HttpContext, RateLimiterStrategyBase<RateLimiterStrategyOptions>>> Policies { get; } = new();

    /// <summary>
    /// Registers a new rate limiting policy in the registry. The policy is associated with a name and a strategy factory.
    /// If a policy with the same name already exists, an <see cref="InvalidOperationException"/> is thrown.
    /// </summary>
    /// <param name="policyName">
    /// A string that uniquely identifies the rate limiting policy.
    /// This name is used to reference the policy when applying it to different API endpoints or parts of the application.
    /// </param>
    /// <param name="strategyFactory">
    /// A factory function that creates an instance of <see cref="RateLimiterStrategyBase{TOptions}"/> representing the rate limiter strategy for the specified policy.
    /// The strategy defines how requests are throttled or allowed based on policy settings.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to register a policy with a name that already exists in the registry.
    /// </exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// registry.RegisterPolicy("MyFixedWindowPolicy", context =>
    /// {
    ///     var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
    ///     var strategy = new FixedWindowRateStrategy(counterStore, new FixedWindowOptions());
    ///     return strategy;
    /// });
    /// </code>
    /// </example>
    public void RegisterPolicy(string policyName, Func<HttpContext, RateLimiterStrategyBase<RateLimiterStrategyOptions>> strategyFactory)
    {
        if (!Policies.TryAdd(policyName, strategyFactory))
        {
            throw new InvalidOperationException($"A policy with the name '{policyName}' already exists.");
        }
        _lastRegisteredPolicy = policyName;
    }

    /// <summary>
    /// Retrieves the rate limiting policy associated with the given policy name.
    /// </summary>
    /// <param name="policyName">
    /// The name of the policy to retrieve.
    /// </param>
    /// <returns>
    /// A factory function that creates an instance of <see cref="RateLimiterStrategyBase{TOptions}"/> representing the rate limiter strategy for the specified policy.
    /// Returns <c>null</c> if no policy with the specified name exists.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// var strategy = registry.GetPolicy("MyPolicy");
    /// if (strategy != null)
    /// {
    ///     // Apply the strategy
    /// }
    /// </code>
    /// </example>
    public Func<HttpContext, RateLimiterStrategyBase<RateLimiterStrategyOptions>>? GetPolicy(string policyName)
    {
        Policies.TryGetValue(policyName, out var value);
        return value;
    }

    /// <summary>
    /// Marks the last registered rate limiting policy as the default policy.
    /// This method sets the default policy for the rate limiter, which will be used if no specific policy is specified.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no policy has been registered yet.
    /// </exception>
    public void SetDefaultPolicy()
    {
        DefaultPolicyName = _lastRegisteredPolicy ?? throw new InvalidOperationException("No policy has been registered yet.");
    }
}