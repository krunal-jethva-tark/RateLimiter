using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies;

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
    /// <param name="key">The key to find from store</param>
    /// <param name="asOfDate">The time requst comes</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether the request is permitted.</returns>
    public override async Task<bool> IsRequestPermittedAsync(string key, DateTime asOfDate)
    {
        var rateLimitData = await _counterStore.GetRateLimitDataAsync(key) ?? new RateLimitData
        {
            Count = 0,
            Expiration = _options.Window,
            CreatedAt = asOfDate,
        };
        if (rateLimitData.Count >= _options.PermitLimit)
        {
            return false;
        }

        rateLimitData.Count++;
        await _counterStore.UpdateRateLimitDataAsync(key, rateLimitData);
        return true;
    }
}