using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies;

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
    /// <param name="key">The HTTP context associated with the request.</param>
    /// <param name="asOfDate"></param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean value indicating whether the request is permitted.</returns>
    public override async Task<bool> IsRequestPermittedAsync(string key, DateTime asOfDate)
    {

        var rateLimitData = await _counterStore.GetRateLimitDataAsync(key) ?? new RateLimitData
        {
            TokensAvailable = _options.BurstCapacity,
            LastRefillTime = asOfDate,
            CreatedAt = asOfDate,
        };
        var elapsedTime = asOfDate - rateLimitData.LastRefillTime;
        var tokensToAdd = (int)(elapsedTime.TotalSeconds * _options.MaxRequestsPerSecond);
        rateLimitData.TokensAvailable = Math.Min(rateLimitData.TokensAvailable + tokensToAdd, _options.BurstCapacity);

        if (rateLimitData.TokensAvailable > 0)
        {
            rateLimitData.TokensAvailable--;
            rateLimitData.LastRefillTime = asOfDate;
            await _counterStore.UpdateRateLimitDataAsync(key, rateLimitData);
            return true; // Request allowed
        }

        return false; // Request denied
    }
}