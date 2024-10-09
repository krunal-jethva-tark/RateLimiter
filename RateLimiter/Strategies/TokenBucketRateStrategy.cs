using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies
{
    /// <summary>
    /// Implementation of the token bucket rate limiting strategy.
    /// </summary>
    public class TokenBucketRateStrategy : RateLimiterStrategyBase<RateLimiterStrategyOptions>
    {
        private readonly IRateLimitCounterStore _counterStore;
        private readonly TokenBucketOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketRateStrategy"/> class.
        /// </summary>
        /// <param name="counterStore">The store used for tracking token availability.</param>
        /// <param name="options">The options for configuring the token bucket rate limiting strategy.</param>
        public TokenBucketRateStrategy(IRateLimitCounterStore counterStore, TokenBucketOptions options)
            : base(counterStore, options)
        {
            _counterStore = counterStore;
            _options = options;
        }

        /// <summary>
        /// Determines if a request is permitted based on the token bucket rate limiting logic.
        /// </summary>
        /// <param name="context">The HTTP context associated with the request.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether the request is permitted.</returns>
        public override async Task<bool> IsRequestPermittedAsync(HttpContext context)
        {
            var key = Options.KeyGenerator(context);

            var (tokensAvailable, lastRefillTime) = await _counterStore.GetTokenBucketStatusAsync(key);
            var elapsedTime = DateTime.UtcNow - lastRefillTime;
            var tokensToAdd = (int)(elapsedTime.TotalSeconds * _options.MaxRequestsPerSecond);
            tokensAvailable = Math.Min(tokensAvailable + tokensToAdd, _options.BurstCapacity); // Ensure we do not exceed burst capacity

            if (tokensAvailable > 0)
            {
                // Consume a token for the current request
                await _counterStore.UpdateTokenBucketAsync(key, tokensAvailable - 1, DateTime.UtcNow);
                return true; // Request allowed
            }

            return false; // Request denied
        }
    }
}
