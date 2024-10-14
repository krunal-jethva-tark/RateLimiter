using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Strategies;

namespace RateLimiter;

/// <summary>
/// The <see cref="RateLimiterPolicyRegistry"/> class is responsible for managing and registering
/// rate limiting policies that control how requests are processed based on defined strategies.
/// </summary>
public class RateLimiterPolicyRegistry
{
    /// <summary>
    /// A dictionary that holds all the registered rate limiting policies.
    /// Each entry in the dictionary maps a policy name to its corresponding strategy factory.
    /// </summary>
    /// <remarks>
    /// The <see cref="Policies"/> dictionary ensures that each policy is identified by a unique name.
    /// The strategy factory is used to create instances of rate limiting strategies based on the current HTTP request context.
    /// </remarks>
    public Dictionary<string, RateLimiterStrategyFactory> Policies { get; } = new();

    /// <summary>
    /// Registers a new rate limiting policy in the registry. The policy is associated with a name and a strategy factory.
    /// If a policy with the same name already exists, an <see cref="InvalidOperationException"/> is thrown.
    /// </summary>
    /// <param name="policyName">
    /// A string that uniquely identifies the rate limiting policy.
    /// This name is used to reference the policy when applying it to different API endpoints or parts of the application.
    /// </param>
    /// <param name="strategyFactory">
    /// A delegate that provides a factory method for creating rate limiting strategies based on the current <see cref="HttpContext"/>.
    /// The strategy defines how requests are throttled or allowed based on policy settings.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to register a policy with a name that already exists in the registry.
    /// </exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// registry.RegisterPolicy("MyPolicy", context =>
    /// {
    ///     var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
    ///     var strategy = new FixedWindowRateStrategy(counterStore, new FixedWindowOptions());
    ///     return (strategy, false);
    /// });
    /// </code>
    /// </example>
    public void RegisterPolicy(string policyName, RateLimiterStrategyFactory strategyFactory)
    {
        if (!Policies.TryAdd(policyName, strategyFactory))
        {
            throw new InvalidOperationException($"A policy with the name '{policyName}' already exists.");
        }
    }

    /// <summary>
    /// Retrieves the rate limiting policy associated with the given policy name.
    /// </summary>
    /// <param name="policyName">
    /// The name of the policy to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="RateLimiterStrategyFactory"/> delegate that creates the rate limiter strategy for the specified policy.
    /// Returns <c>null</c> if no policy with the specified name exists.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// var strategyFactory = registry.GetPolicy("MyPolicy");
    /// if (strategyFactory != null)
    /// {
    ///     var (strategy, isGlobal) = strategyFactory(context);
    ///     // Apply the strategy
    /// }
    /// </code>
    /// </example>
    public RateLimiterStrategyFactory? GetPolicy(string policyName)
    {
        Policies.TryGetValue(policyName, out var value);
        return value;
    }
}

/// <summary>
/// Represents a factory method that returns a rate limiting strategy and a flag indicating
/// whether the strategy is global, based on the provided <see cref="HttpContext"/>.
/// </summary>
/// <param name="context">
/// The <see cref="HttpContext"/> of the current request, used to create the appropriate rate limiter strategy.
/// </param>
/// <returns>
/// A tuple where the first value is an instance of a rate limiting strategy derived from 
/// <see cref="RateLimiterStrategyBase{TOptions}"/> and the second value is a boolean indicating
/// whether the policy is applied globally across all endpoints.
/// </returns>
public delegate (RateLimiterStrategyBase<RateLimiterStrategyOptions> strategy, bool IsGlobal) RateLimiterStrategyFactory(HttpContext context);