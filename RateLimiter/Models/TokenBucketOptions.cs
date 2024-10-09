namespace RateLimiter.Models
{
    /// <summary>
    /// Options for configuring a token bucket rate limiting strategy.
    /// </summary>
    public class TokenBucketOptions : RateLimiterStrategyOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of requests that can be processed per second.
        /// </summary>
        /// <value>
        /// An integer representing the maximum requests allowed per second.
        /// </value>
        public int MaxRequestsPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the burst capacity for the token bucket.
        /// This defines the maximum number of requests that can be made in a burst.
        /// </summary>
        /// <value>
        /// An integer representing the maximum number of requests allowed in a burst.
        /// </value>
        public int BurstCapacity { get; set; }
    }
}