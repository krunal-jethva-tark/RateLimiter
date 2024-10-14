using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies;

/// <summary>
/// Serves as the base class for implementing various rate limiting strategies.
/// Derived classes must implement the <see cref="IsRequestPermittedAsync"/> method, 
/// which defines how rate limiting is applied based on the strategy's options and logic.
/// </summary>
/// <typeparam name="TOptions">
/// The type of options used for configuring the rate limiting strategy. 
/// This should inherit from <see cref="RateLimiterStrategyOptions"/> and include specific configuration details 
/// relevant to the rate limiting strategy (e.g., window size, request limits).
/// </typeparam>
public abstract class RateLimiterStrategyBase<TOptions>
    where TOptions : RateLimiterStrategyOptions
{
    /// <summary>
    /// Gets the options used for configuring the rate limiting strategy.
    /// </summary>
    /// <remarks>
    /// The options contain the configurable settings for the specific rate limiting strategy. 
    /// This includes parameters like time windows, token generation rates, and request limits.
    /// </remarks>
    public TOptions Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiterStrategyBase{TOptions}"/> class.
    /// </summary>
    /// <param name="counterStore">
    /// The <see cref="IRateLimitCounterStore"/> used for tracking and persisting request counts 
    /// for rate limiting. This store can be backed by in-memory, distributed cache, or database storage.
    /// </param>
    /// <param name="options">
    /// The configuration options for the rate limiting strategy, which dictate how requests are counted, 
    /// limited, and throttled.
    /// </param>
    protected RateLimiterStrategyBase(IRateLimitCounterStore counterStore, TOptions options)
    {
        Options = options;
    }

    /// <summary>
    /// Determines whether a request is permitted according to the rate limiting rules.
    /// This method must be implemented by derived classes to specify the logic for evaluating requests.
    /// </summary>
    /// <param name="key">
    /// A unique identifier (usually tied to the client's IP address or user ID) that represents
    /// the entity whose request is being evaluated.
    /// </param>
    /// <param name="asOfDate">
    /// The current date and time used to evaluate the rate limiting window or period. 
    /// This ensures that rate limiting is applied based on a consistent time reference.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a boolean value indicating 
    /// whether the request is permitted (<c>true</c>) or rejected due to exceeding the limit (<c>false</c>).
    /// </returns>
    /// <example>
    /// Example usage in a derived class:
    /// <code>
    /// public override async Task&lt;bool&gt; IsRequestPermittedAsync(string key, DateTime asOfDate)
    /// {
    ///     var currentCount = await _counterStore.GetRequestCountAsync(key, asOfDate);
    ///     return currentCount &lt; Options.RequestLimit;
    /// }
    /// </code>
    /// </example>
    public abstract Task<bool> IsRequestPermittedAsync(string key, DateTime asOfDate);
}