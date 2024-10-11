using RateLimiter.Models;

namespace RateLimiter.Stores;

/// <summary>
/// Interface for managing rate limit counters and token bucket statuses.
/// </summary>
public interface IRateLimitCounterStore
{
    /// <summary>
    /// Retrieves the current rate limit data for a specified key.
    /// </summary>
    /// <param name="key">The key for which to retrieve the rate limit data.</param>
    /// <returns>A task representing the asynchronous operation, containing the rate limit data.</returns>
    Task<RateLimitData?> GetRateLimitDataAsync(string key);

    /// <summary>
    /// Updates the rate limit data for a specified key.
    /// </summary>
    /// <param name="key">The key for which to update the rate limit data.</param>
    /// <param name="data">The updated rate limit data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateRateLimitDataAsync(string key, RateLimitData data);
}