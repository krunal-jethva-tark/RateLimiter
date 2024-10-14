using RateLimiter.Models;

namespace RateLimiter.Stores;

/// <summary>
/// Defines methods for managing rate limit counters and token bucket statuses in a storage mechanism.
/// This interface is typically implemented by classes that manage in-memory, distributed cache, or database-based storage.
/// </summary>
public interface IRateLimitCounterStore
{
    /// <summary>
    /// Asynchronously retrieves and updates the rate limit data for a specified key in the storage mechanism. 
    /// The key typically represents a unique identifier for the rate-limited entity, such as a client IP address or user ID.
    /// The update logic is applied to the current rate limit data for the specified key.
    /// </summary>
    /// <param name="key">
    /// The unique key used to identify the entity whose rate limit data is being retrieved and updated. 
    /// Typically corresponds to a client IP address, user ID, or similar identifier.
    /// </param>
    /// <param name="asOfDate">The current date and time when the request is made, used to determine the active window period.</param>
    /// <param name="updateLogic">
    /// A delegate (function) that takes the current <see cref="RateLimitData"/> (or <c>null</c> if none exists)
    /// and returns the updated <see cref="RateLimitData"/>. This logic is used to update the rate limit data 
    /// (e.g., increment request counts or refill tokens).
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, which contains the updated <see cref="RateLimitData"/> object 
    /// associated with the specified key.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// var updatedData = await counterStore.GetAndUpdateRateLimitDataAsync("client-key", existingData => 
    /// {
    ///     if (existingData == null)
    ///     {
    ///         return new RateLimitData
    ///         {
    ///             Count = 1,
    ///             Expiration = TimeSpan.FromMinutes(1),
    ///             TokensAvailable = 10,
    ///             LastRefillTime = DateTime.UtcNow
    ///         };
    ///     }
    ///     else
    ///     {
    ///         existingData.Count++;
    ///         return existingData;
    ///     }
    /// });
    /// </code>
    /// </example>
    Task<RateLimitData> GetAndUpdateRateLimitDataAsync(string key, DateTime asOfDate,
        Func<RateLimitData?, DateTime, RateLimitData> updateLogic);
}