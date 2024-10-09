namespace RateLimiter.Stores
{
    /// <summary>
    /// Interface for managing rate limit counters and token bucket statuses.
    /// </summary>
    public interface IRateLimitCounterStore
    {
        /// <summary>
        /// Retrieves the current request count for a specified key.
        /// </summary>
        /// <param name="key">The key for which to retrieve the request count.</param>
        /// <returns>A task that represents the asynchronous operation, containing the request count.</returns>
        Task<int> GetRequestCountAsync(string key);

        /// <summary>
        /// Increments the request count for a specified key and sets the expiration time.
        /// </summary>
        /// <param name="key">The key for which to increment the request count.</param>
        /// <param name="expiration">The duration for which the request count is valid.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task IncrementRequestCountAsync(string key, TimeSpan expiration);

        /// <summary>
        /// Retrieves the current status of the token bucket for a specified key.
        /// </summary>
        /// <param name="key">The key for which to retrieve the token bucket status.</param>
        /// <returns>A task that represents the asynchronous operation, containing the number of tokens available and the last refill time.</returns>
        Task<(int tokenAvailable, DateTime lastRefillTime)> GetTokenBucketStatusAsync(string key);

        /// <summary>
        /// Updates the status of the token bucket for a specified key.
        /// </summary>
        /// <param name="key">The key for which to update the token bucket status.</param>
        /// <param name="tokensAvailable">The number of tokens available in the bucket.</param>
        /// <param name="lastRefillTime">The last time the token bucket was refilled.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateTokenBucketAsync(string key, int tokensAvailable, DateTime lastRefillTime);
    }
}
