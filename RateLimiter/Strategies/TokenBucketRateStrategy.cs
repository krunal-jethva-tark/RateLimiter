using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Strategies;

/// <summary>
/// Implements the token bucket rate limiting strategy, which allows a burst of requests
/// and then refills tokens over time at a defined rate. Each request consumes a token,
/// and once the tokens are exhausted, further requests are denied until tokens are replenished.
/// </summary>
public class TokenBucketRateStrategy : RateLimiterStrategyBase<RateLimiterStrategyOptions>
{
    private readonly IRateLimitCounterStore _counterStore;
    private readonly TokenBucketOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenBucketRateStrategy"/> class.
    /// </summary>
    /// <param name="counterStore">
    /// The store used for tracking token availability, storing the number of tokens available
    /// and the last time the bucket was refilled. Typically backed by in-memory, distributed cache, or a database.
    /// </param>
    /// <param name="options">
    /// The <see cref="TokenBucketOptions"/> used to configure the token bucket rate limiting strategy.
    /// These options include burst capacity, token refill rate, and maximum requests per second.
    /// </param>
    public TokenBucketRateStrategy(IRateLimitCounterStore counterStore, TokenBucketOptions options)
        : base(counterStore, options)
    {
        _counterStore = counterStore;
        _options = options;
    }

    /// <summary>
    /// Determines if a request is permitted based on the token bucket rate limiting logic.
    /// A request is allowed if there are tokens available in the bucket, and the tokens are refilled
    /// over time based on the configured rate.
    /// </summary>
    /// <param name="key">
    /// The unique identifier for the requestor, typically based on a client IP address or user ID.
    /// This key is used to retrieve and track token data from the counter store.
    /// </param>
    /// <param name="asOfDate">
    /// The current date and time when the request is made. Used to calculate the elapsed time since the last refill.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a boolean value indicating
    /// whether the request is permitted (<c>true</c>) or rejected (<c>false</c>) based on the token availability
    /// and a <see cref="RateLimitResponseHeaders"/> object containing the rate limit headers.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// var (isPermitted, headers) = await tokenBucketStrategy.IsRequestPermittedAsync("client-key", DateTime.UtcNow);
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
    public override async Task<(bool, RateLimitResponseHeaders)> IsRequestPermittedAsync(string key, DateTime asOfDate)
    {
        var rateLimitData = await _counterStore.GetAndUpdateRateLimitDataAsync(key, asOfDate, UpdateLogic);
        var isPermitted = rateLimitData.TokensAvailable >= 0;
        
        var headers = new RateLimitResponseHeaders
        {
            Limit = _options.MaxRequestsPerSecond.ToString(),
            Remaining = rateLimitData.TokensAvailable.ToString(),
            Reset = (rateLimitData.LastRefillTime + TimeSpan.FromSeconds(1)).ToString("o")
        };
        return (isPermitted, headers);
    }

    private RateLimitData UpdateLogic(RateLimitData? rateLimitData, DateTime asOfDate)
    {
        rateLimitData ??= new RateLimitData {
            TokensAvailable = _options.MaxRequestsPerSecond,
            LastRefillTime = asOfDate,
            CreatedAt = asOfDate,
        };

        var elapsedTime = asOfDate - rateLimitData.LastRefillTime;
        var tokensToAdd = (int)(elapsedTime.TotalSeconds * _options.MaxRequestsPerSecond);
        rateLimitData.TokensAvailable = Math.Min(rateLimitData.TokensAvailable + tokensToAdd, _options.BurstCapacity);
        if (rateLimitData.TokensAvailable >= 0)
        {
            rateLimitData.TokensAvailable--;
        }                                
        rateLimitData.LastRefillTime = asOfDate;
        return rateLimitData;
    }
}