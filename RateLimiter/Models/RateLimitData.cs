namespace RateLimiter.Models;

/// <summary>
/// Represents the data used for rate limiting strategies.
/// This class encapsulates the necessary data to track requests and manage rate limiting across different strategies.
/// </summary>
public class RateLimitData
{
    /// <summary>
    /// Gets or sets the count for Fixed Window or other counting strategies.
    /// This property tracks the number of requests made within a specific time period.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the number of tokens available for Token Bucket or Leaky Bucket strategies.
    /// This property indicates how many tokens are currently available for processing requests.
    /// </summary>
    public int TokensAvailable { get; set; }

    /// <summary>
    /// Gets or sets the last refill time for strategies like Token Bucket.
    /// This timestamp indicates when the last refill of tokens occurred, used for calculating token availability.
    /// </summary>
    public DateTime LastRefillTime { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for data, such as in Fixed Window strategies.
    /// This property defines how long the rate limit data is valid before it expires.
    /// </summary>
    public TimeSpan Expiration { get; set; }
    
    /// <summary>
    /// Gets or sets the creation time of the rate limit data.
    /// This timestamp is used for tracking when the rate limit data was first created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}