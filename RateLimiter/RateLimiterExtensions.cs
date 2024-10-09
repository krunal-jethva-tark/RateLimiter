using RateLimiter.Models;
using RateLimiter.Stores;
using RateLimiter.Strategies;

namespace RateLimiter
{
    /// <summary>
    /// Provides extension methods for registering rate limiting policies.
    /// </summary>
    public static class RateLimiterExtensions
    {
        /// <summary>
        /// Registers a fixed window rate limiting policy in the provided policy registry.
        /// </summary>
        /// <param name="registry">The policy registry to register the policy with.</param>
        /// <param name="policyName">The name of the policy to be registered.</param>
        /// <param name="configure">A delegate to configure the options for the fixed window policy.</param>
        public static void AddFixedWindowPolicy(this RateLimiterPolicyRegistry registry, string policyName, Action<FixedWindowOptions> configure)
        {
            var options = new FixedWindowOptions();
            configure(options);
            registry.RegisterPolicy(policyName, context =>
            {
                var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
                return new FixedWindowRateStrategy(counterStore, options);
            });
        }

        /// <summary>
        /// Registers a token bucket rate limiting policy in the provided policy registry.
        /// </summary>
        /// <param name="registry">The policy registry to register the policy with.</param>
        /// <param name="policyName">The name of the policy to be registered.</param>
        /// <param name="configure">A delegate to configure the options for the token bucket policy.</param>
        public static void AddTokenBucketPolicy(this RateLimiterPolicyRegistry registry, string policyName, Action<TokenBucketOptions> configure)
        {
            var options = new TokenBucketOptions();
            configure(options);

            registry.RegisterPolicy(policyName, context => 
            {
                var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
                return new TokenBucketRateStrategy(counterStore, options);
            });
        }
    }
}
