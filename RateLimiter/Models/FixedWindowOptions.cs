namespace RateLimiter.Models;

/// <summary>
/// Options for configuring a fixed window rate limiting strategy.
/// </summary>
public class FixedWindowOptions : RateLimiterStrategyOptions
{
    /// <summary>
    /// Gets or sets the maximum number of permitted requests within the specified time window.
    /// </summary>
    /// <value>
    /// An integer representing the limit of requests that can be made within the defined window.
    /// </value>
    public int PermitLimit { get; set; }

    /// <summary>
    /// Gets or sets the duration of the time window during which the requests are counted.
    /// </summary>
    /// <value>
    /// A <see cref="TimeSpan"/> representing the length of the time window for rate limiting.
    /// </value>
    public TimeSpan Window { get; set; }
}