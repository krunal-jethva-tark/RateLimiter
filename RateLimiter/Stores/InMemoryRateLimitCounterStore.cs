using System.Collections.Concurrent;
using RateLimiter.Models;

namespace RateLimiter.Stores;

/// <summary>
/// An in-memory implementation of the <see cref="IRateLimitCounterStore"/> interface.
/// This class stores rate limiting data in a thread-safe, in-memory <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// Data stored in-memory is lost when the application restarts, making this suitable for lightweight or single-instance applications.
/// </summary>
public class InMemoryRateLimitCounterStore : IRateLimitCounterStore
{
    private readonly ConcurrentDictionary<string, RateLimitData> _store = new();
    private readonly ConcurrentDictionary<string, object> _lockStore = new();

    /// <summary>
    /// Retrieves and updates the rate limit data for a specified key asynchronously with distributed locking.
    /// This method allows for injecting custom logic to modify the rate limit data and then updates the data in Redis.
    /// </summary>
    /// <param name="key">The key for which to retrieve and update the rate limit data.</param>
    /// <param name="asOfDate">The current date and time when the request is made, used to determine the active window period.</param>
    /// <param name="updateLogic">A function that takes the current rate limit data and returns the updated data.</param>
    /// <returns>A task representing the asynchronous operation, containing the updated <see cref="RateLimitData"/>.</returns>
    public Task<RateLimitData> GetAndUpdateRateLimitDataAsync(string key, DateTime asOfDate,
        Func<RateLimitData?, DateTime, RateLimitData> updateLogic)
    {
        // Use a lock object for the specific key
        var lockObject = _lockStore.GetOrAdd(key, _ => new object());
        lock (lockObject)
        {
            RateLimitData? rateLimitData = null;
            var now = DateTime.UtcNow;
            if (_store.TryGetValue(key, out var data))
            {
                if (data.CreatedAt.Add(data.Expiration) > asOfDate)
                {
                    rateLimitData = data;
                }
            }

            rateLimitData = updateLogic(rateLimitData, asOfDate);
            _store.AddOrUpdate(key, rateLimitData, (_, existingData) =>
            {
                // Update with new data
                existingData.Count = rateLimitData.Count;
                existingData.TokensAvailable = rateLimitData.TokensAvailable;
                existingData.LastRefillTime = rateLimitData.LastRefillTime;
                existingData.Expiration = rateLimitData.Expiration;
                existingData.CreatedAt = rateLimitData.CreatedAt;
                return existingData;
            });

            return Task.FromResult(rateLimitData);
        }
    }
}