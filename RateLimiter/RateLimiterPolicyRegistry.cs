using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Strategies;

namespace RateLimiter
{
    /// <summary>
    /// Registers and manages rate limiting policies.
    /// </summary>
    public class RateLimiterPolicyRegistry
    {
        /// <summary>
        /// A dictionary that holds registered policies and their associated strategy factories.
        /// </summary>
        public Dictionary<string, Func<HttpContext, RateLimiterStrategyBase<RateLimiterStrategyOptions>>> Policies { get; } = new();

        /// <summary>
        /// Registers a new rate limiting policy with the specified name and strategy factory.
        /// </summary>
        /// <param name="policyName">The name of the policy to register.</param>
        /// <param name="strategyFactory">A factory method that creates a rate limiter strategy based on the current HTTP context.</param>
        /// <exception cref="InvalidOperationException">Thrown when a policy with the same name already exists.</exception>
        public void RegisterPolicy(string policyName, Func<HttpContext, RateLimiterStrategyBase<RateLimiterStrategyOptions>> strategyFactory)
        {
            if (!Policies.TryAdd(policyName, strategyFactory))
            {
                throw new InvalidOperationException($"A policy with the name '{policyName}' already exists.");
            }
        }
    }
}