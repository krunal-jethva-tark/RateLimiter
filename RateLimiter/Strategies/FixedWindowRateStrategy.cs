using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies;

/// <summary>
/// Implements the fixed window rate limiting strategy, where the request count is reset
/// at the start of each fixed time window. This strategy allows a fixed number of requests
/// within a specified window period.
/// </summary>
public class FixedWindowRateStrategy : RateLimiterStrategyBase<RateLimiterStrategyOptions>
{
    private readonly IRateLimitCounterStore _counterStore;
    private readonly FixedWindowOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedWindowRateStrategy"/> class.
    /// </summary>
    /// <param name="counterStore">
    /// The store used for tracking and persisting request counts and rate limiting data.
    /// Typically backed by in-memory, distributed cache, or a database.
    /// </param>
    /// <param name="options">
    /// The <see cref="FixedWindowOptions"/> used for configuring the fixed window rate limiting strategy.
    /// These options include the window size (time period) and the maximum number of permitted requests within that period.
    /// </param>
    public FixedWindowRateStrategy(IRateLimitCounterStore counterStore, FixedWindowOptions options)
        : base(counterStore, options)
    {
        _counterStore = counterStore;
        _options = options;
    }

    /// <summary>
    /// Determines if a request is permitted based on the fixed window rate limiting logic.
    /// If the request count exceeds the permitted limit for the window, the request will be denied.
    /// </summary>
    /// <param name="key">
    /// The unique identifier for the requestor, typically based on a client IP address or user ID.
    /// This key is used to retrieve and track rate limit data from the counter store.
    /// </param>
    /// <param name="asOfDate">
    /// The current date and time when the request is made, used to determine the active window period.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a boolean value indicating
    /// whether the request is permitted (<c>true</c>) or rejected (<c>false</c>) based on the rate limit.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// var isPermitted = await fixedWindowStrategy.IsRequestPermittedAsync("client-key", DateTime.UtcNow);
    /// if (isPermitted)
    /// {
    ///     // Allow the request to proceed
    /// }
    /// else
    /// {
    ///     // Reject the request, rate limit exceeded
    /// }
    /// </code>
    /// </example>
    public override async Task<bool> IsRequestPermittedAsync(string key, DateTime asOfDate)
    {
        var rateLimitData = await _counterStore.GetAndUpdateRateLimitDataAsync(key, asOfDate, UpdateLogic);
        return rateLimitData.Count <= _options.PermitLimit;
    }

    private RateLimitData UpdateLogic(RateLimitData? rateLimitData, DateTime asOfDate)
    {
        rateLimitData ??= new RateLimitData
        {
            Count = 0,
            Expiration = _options.Window,
            CreatedAt = asOfDate,
        };
        rateLimitData.Count++;
        return rateLimitData;
    }
}