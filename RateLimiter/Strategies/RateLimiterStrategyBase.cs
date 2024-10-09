using Microsoft.AspNetCore.Http;
using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies
{
    /// <summary>
    /// Base class for implementing rate limiting strategies.
    /// </summary>
    /// <typeparam name="TOptions">The type of options used for configuring the rate limiting strategy.</typeparam>
    public abstract class RateLimiterStrategyBase<TOptions>
        where TOptions : RateLimiterStrategyOptions
    {
        /// <summary>
        /// Gets the options used for configuring the rate limiting strategy.
        /// </summary>
        public TOptions Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimiterStrategyBase{TOptions}"/> class.
        /// </summary>
        /// <param name="counterStore">The store used for tracking request counts.</param>
        /// <param name="options">The options for configuring the rate limiting strategy.</param>
        protected RateLimiterStrategyBase(IRateLimitCounterStore counterStore, TOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Determines if a request is permitted based on the rate limiting strategy.
        /// </summary>
        /// <param name="context">The HTTP context associated with the request.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether the request is permitted.</returns>
        public abstract Task<bool> IsRequestPermittedAsync(HttpContext context);
    }
}