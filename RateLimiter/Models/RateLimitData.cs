namespace RateLimiter.Models;

/// <summary>
/// Represents the data used for rate limiting strategies.
/// </summary>
public class RateLimitData
{
    /// <summary>
    /// Gets or sets the count for Fixed Window or other counting strategies.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the number of tokens available for Token Bucket or Leaky Bucket strategies.
    /// </summary>
    public int TokensAvailable { get; set; }

    /// <summary>
    /// Gets or sets the last refill time for strategies like Token Bucket.
    /// </summary>
    public DateTime LastRefillTime { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for data, such as in Fixed Window strategies.
    /// </summary>
    public TimeSpan Expiration { get; set; }
    public required DateTime CreatedAt { get; set; }
}
