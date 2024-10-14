using Newtonsoft.Json;
using RateLimiter.Models;
using RateLimiter.Stores;
using StackExchange.Redis;

namespace RateLimiter.Redis;

/// <summary>
/// Implementation of the <see cref="IRateLimitCounterStore"/> interface using Redis as the storage backend.
/// This class manages rate limit data using a Redis database, providing persistence and distributed access.
/// </summary>
public class RedisRateLimitCounterStore(IConnectionMultiplexer redisConnection) : IRateLimitCounterStore
{
    private readonly IDatabase _redisDatabase = redisConnection.GetDatabase();

    /// <summary>
    /// Retrieves the current rate limit data for a specified key asynchronously.
    /// If the key does not exist or the data is null, <c>null</c> is returned.
    /// </summary>
    /// <param name="key">The key for which to retrieve the rate limit data.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the <see cref="RateLimitData"/> for the specified key,
    /// or <c>null</c> if no valid data exists.
    /// </returns>
    public async Task<RateLimitData?> GetRateLimitDataAsync(string key)
    {
        var data = await _redisDatabase.StringGetAsync(key);
        return data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<RateLimitData>(data);
    }
    
    /// <summary>
    /// Updates the rate limit data for a specified key asynchronously.
    /// The data is serialized to JSON and stored in Redis with an expiration time.
    /// </summary>
    /// <param name="key">The key for which to update the rate limit data.</param>
    /// <param name="data">The updated <see cref="RateLimitData"/> object containing the latest rate limiting information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateRateLimitDataAsync(string key, RateLimitData data)
    {
        var serializedData = JsonConvert.SerializeObject(data);
        return _redisDatabase.StringSetAsync(key, serializedData, data.Expiration);
    }
}