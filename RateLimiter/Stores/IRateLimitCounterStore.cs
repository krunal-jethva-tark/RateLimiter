using RateLimiter.Models;

namespace RateLimiter.Stores;

/// <summary>
/// Defines methods for managing rate limit counters and token bucket statuses in a storage mechanism.
/// This interface is typically implemented by classes that manage in-memory, distributed cache, or database-based storage.
/// </summary>
public interface IRateLimitCounterStore
{
    /// <summary>
    /// Asynchronously retrieves the current rate limit data for a specified key.
    /// The key typically represents a unique identifier for the rate-limited entity, such as a client IP address or user ID.
    /// </summary>
    /// <param name="key">
    /// The unique key used to identify the entity whose rate limit data is being retrieved. 
    /// This key is typically derived from the request, such as the client’s IP address or user-specific identifier.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, which contains the <see cref="RateLimitData"/> associated with the specified key.
    /// Returns <c>null</c> if no data exists for the key (e.g., first request for this key).
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// var rateLimitData = await counterStore.GetRateLimitDataAsync("client-key");
    /// if (rateLimitData == null)
    /// {
    ///     // Handle new request with no prior rate limit data
    /// }
    /// </code>
    /// </example>
    Task<RateLimitData?> GetRateLimitDataAsync(string key);

    /// <summary>
    /// Asynchronously updates the rate limit data for a specified key in the storage mechanism.
    /// This method is called after changes to the rate limit data, such as incrementing request counts or refilling tokens.
    /// </summary>
    /// <param name="key">
    /// The unique key used to identify the entity whose rate limit data is being updated.
    /// Typically corresponds to a client IP address, user ID, or similar identifier.
    /// </param>
    /// <param name="data">
    /// The updated <see cref="RateLimitData"/> object containing the latest rate limiting information, 
    /// such as request counts, token availability, or window expiration.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// This task completes once the rate limit data is successfully updated in the storage.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// var rateLimitData = new RateLimitData
    /// {
    ///     Count = 5,
    ///     Expiration = TimeSpan.FromMinutes(1),
    ///     TokensAvailable = 10,
    ///     LastRefillTime = DateTime.UtcNow
    /// };
    /// await counterStore.UpdateRateLimitDataAsync("client-key", rateLimitData);
    /// </code>
    /// </example>
    Task UpdateRateLimitDataAsync(string key, RateLimitData data);
}