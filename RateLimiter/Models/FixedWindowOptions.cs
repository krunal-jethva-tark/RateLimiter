namespace RateLimiter.Models;

/// <summary>
/// Represents the options for configuring a fixed window rate limiting strategy.
/// This class contains properties that define the limits and time windows for the fixed window strategy.
/// </summary>
public class FixedWindowOptions : RateLimiterStrategyOptions
{
    /// <summary>
    /// Gets or sets the maximum number of permitted requests within the specified time window.
    /// This limit controls how many requests a user can make in the defined time period.
    /// </summary>
    /// <value>
    /// An integer representing the limit of requests that can be made within the defined window.
    /// </value>
    public int PermitLimit { get; set; }

    /// <summary>
    /// Gets or sets the duration of the time window during which the requests are counted.
    /// This specifies how long the rate limiting period lasts.
    /// </summary>
    /// <value>
    /// A <see cref="TimeSpan"/> representing the length of the time window for rate limiting.
    /// </value>
    public TimeSpan Window { get; set; }
}