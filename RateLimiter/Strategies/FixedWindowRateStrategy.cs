using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies
{
    /// <summary>
    /// Implementation of the fixed window rate limiting strategy.
    /// </summary>
    public class FixedWindowRateStrategy : RateLimiterStrategyBase<RateLimiterStrategyOptions>
    {
        private readonly IRateLimitCounterStore _counterStore;
        private readonly FixedWindowOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedWindowRateStrategy"/> class.
        /// </summary>
        /// <param name="counterStore">The store used for tracking request counts.</param>
        /// <param name="options">The options for configuring the fixed window rate limiting strategy.</param>
        public FixedWindowRateStrategy(IRateLimitCounterStore counterStore, FixedWindowOptions options)
            : base(counterStore, options)
        {
            _counterStore = counterStore;
            _options = options;
        }

        /// <summary>
        /// Determines if a request is permitted based on the fixed window rate limiting logic.
        /// </summary>
        /// <param name="context">The HTTP context associated with the request.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether the request is permitted.</returns>
        public override async Task<bool> IsRequestPermittedAsync(HttpContext context)
        {
            var key = Options.KeyGenerator(context);
            var requestCount = await _counterStore.GetRequestCountAsync(key);

            if (requestCount >= _options.PermitLimit)
            {
                return false; // Request limit exceeded
            }

            await _counterStore.IncrementRequestCountAsync(key, _options.Window);
            
            return true; // Request permitted
        }
    }
}
