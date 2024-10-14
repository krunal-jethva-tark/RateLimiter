namespace RateLimiter.Models;

/// <summary>
/// Options for configuring a token bucket rate limiting strategy.
/// This class allows you to specify the rate limits for requests using a token bucket algorithm.
/// </summary>
public class TokenBucketOptions : RateLimiterStrategyOptions
{
    /// <summary>
    /// Gets or sets the maximum number of requests that can be processed per second.
    /// This limit controls the rate at which tokens are consumed from the bucket.
    /// </summary>
    /// <value>
    /// An integer representing the maximum requests allowed per second.
    /// This value dictates how quickly clients can send requests to the server.
    /// </value>
    public int MaxRequestsPerSecond { get; set; }

    /// <summary>
    /// Gets or sets the burst capacity for the token bucket.
    /// This defines the maximum number of requests that can be made in a burst, allowing for sudden spikes in traffic.
    /// </summary>
    /// <value>
    /// An integer representing the maximum number of requests allowed in a burst.
    /// This value determines how many tokens can be consumed at once during high demand.
    /// </value>
    public int BurstCapacity { get; set; }
}