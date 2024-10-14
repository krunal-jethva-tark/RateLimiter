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
    private const string LockSuffix = ":lock";
    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Retrieves and updates the rate limit data for a specified key asynchronously with distributed locking.
    /// This method allows for injecting custom logic to modify the rate limit data and then updates the data in Redis.
    /// </summary>
    /// <param name="key">The key for which to retrieve and update the rate limit data.</param>
    /// <param name="asOfDate">The current date and time when the request is made, used to determine the active window period.</param>
    /// <param name="updateLogic">A function that takes the current rate limit data and returns the updated data.</param>
    /// <returns>A task representing the asynchronous operation, containing the updated <see cref="RateLimitData"/>.</returns>
    public async Task<RateLimitData> GetAndUpdateRateLimitDataAsync(string key, DateTime asOfDate,
        Func<RateLimitData?, DateTime, RateLimitData> updateLogic)
    {
        var lockKey = key + LockSuffix;
        var lockValue = Guid.NewGuid().ToString();

        if (!await _redisDatabase.StringSetAsync(lockKey, lockValue, _lockTimeout, When.NotExists))
        {
            return new RateLimitData{ CreatedAt = DateTime.UtcNow, Count = int.MaxValue };
        }

        try
        {
            var data = await _redisDatabase.StringGetAsync(key);
            var rateLimitData = data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<RateLimitData>(data);

            // Apply custom logic to update the data
            rateLimitData = updateLogic(rateLimitData, asOfDate);

            // Serialize and save the updated data
            var serializedData = JsonConvert.SerializeObject(rateLimitData);
            await _redisDatabase.StringSetAsync(key, serializedData, rateLimitData.Expiration);

            // Return the updated data
            return rateLimitData;
        }
        finally
        {
            // Release the lock if we still hold it
            if (await _redisDatabase.StringGetAsync(lockKey) == lockValue)
            {
                await _redisDatabase.KeyDeleteAsync(lockKey);
            }
        }
    }
}