using System.Collections.Concurrent;
using RateLimiter.Models;

namespace RateLimiter.Stores;

/// <summary>
/// An in-memory implementation of the <see cref="IRateLimitCounterStore"/> interface.
/// This class stores rate limiting data in a thread-safe, in-memory <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// Data stored in-memory is lost when the application restarts, making this suitable for lightweight or single-instance applications.
/// </summary>
public class InMemoryRateLimitCounterStore: IRateLimitCounterStore
{
    private readonly ConcurrentDictionary<string, RateLimitData> _store = new();
    private readonly ConcurrentDictionary<string, object> _lockStore = new();
    
    /// <summary>
    /// Retrieves the rate limit data for the specified key asynchronously.
    /// If the rate limit data has expired based on the expiration time, <c>null</c> is returned.
    /// </summary>
    /// <param name="key">The unique key identifying the client or entity whose rate limit data is to be retrieved.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the <see cref="RateLimitData"/> for the specified key,
    /// or <c>null</c> if no valid data exists (e.g., data has expired).
    /// </returns>
    public Task<RateLimitData?> GetRateLimitDataAsync(string key)
    {
        // Use a lock object for the specific key
        var lockObject = _lockStore.GetOrAdd(key, _ => new object());

        RateLimitData? rateLimitData = null;
        lock (lockObject)
        {
            if (_store.TryGetValue(key, out var data))
            {
                var now = DateTime.UtcNow;
                if (data.CreatedAt.Add(data.Expiration) > now)
                {
                    rateLimitData = data;
                }
            }
        }
        
        return Task.FromResult<RateLimitData?>(rateLimitData);
    }

    /// <summary>
    /// Updates the rate limit data for the specified key asynchronously.
    /// If the key already exists in the dictionary, the existing data is updated; otherwise, a new entry is added.
    /// </summary>
    /// <param name="key">The unique key identifying the client or entity whose rate limit data is to be updated.</param>
    /// <param name="data">The updated <see cref="RateLimitData"/> object containing the latest rate limiting information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateRateLimitDataAsync(string key, RateLimitData data)
    {
        // Use a lock object for the specific key
        var lockObject = _lockStore.GetOrAdd(key, _ => new object());

        lock (lockObject)
        {
            _store.AddOrUpdate(key, data, (existingKey, existingData) =>
            {
                // Update with new data
                existingData.Count = data.Count;
                existingData.TokensAvailable = data.TokensAvailable;
                existingData.LastRefillTime = data.LastRefillTime;
                existingData.Expiration = data.Expiration;
                existingData.CreatedAt = DateTime.UtcNow;
                return existingData;
            });
        }

        return Task.CompletedTask;
    }
}