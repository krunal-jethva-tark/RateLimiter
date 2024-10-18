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
    private readonly TimeSpan _lockExpiry = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _retryDelay = TimeSpan.FromMilliseconds(200);

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
        var lockKey = $"{key}{LockSuffix}";
        var lockValue = Guid.NewGuid().ToString();
        bool lockAcquired = false;
        var startTime = DateTime.UtcNow;
        try
        {
            while (!lockAcquired)
            {
                lockAcquired = await _redisDatabase.LockTakeAsync(lockKey, lockValue, _lockExpiry);
                if (lockAcquired)
                    break; // Lock acquired, exit loop

                if (DateTime.UtcNow - startTime > _lockExpiry)
                {
                    throw new Exception($"Could not acquire lock for key: {key} after waiting {_lockTimeout.TotalSeconds} seconds.");
                }
                
                // wait before trying again
                await Task.Delay(_retryDelay);
            }
            
            var data = await _redisDatabase.StringGetAsync(key);
            var rateLimitData = data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<RateLimitData>(data);
            if (rateLimitData is not null && rateLimitData.CreatedAt.Add(rateLimitData.Expiration) < asOfDate)
            {
                rateLimitData = null;
            }

            // Apply custom logic to update the data
            rateLimitData = updateLogic(rateLimitData, asOfDate);

            // Serialize and save the updated data
            var serializedData = JsonConvert.SerializeObject(rateLimitData);
            var updated = await _redisDatabase.StringSetAsync(key, serializedData, null, When.Exists);
            if (!updated)
            {
                await _redisDatabase.StringSetAsync(key, serializedData, rateLimitData.Expiration);
            }

            // Return the updated data
            return rateLimitData;
        }
        finally
        {
            // Release the lock if we still hold it
            if (lockAcquired)
            {
                await _redisDatabase.LockReleaseAsync(lockKey, lockValue);
            }
        }
    }
}