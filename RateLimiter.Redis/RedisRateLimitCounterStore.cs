using Newtonsoft.Json;
using RateLimiter.Models;
using RateLimiter.Stores;
using StackExchange.Redis;

namespace RateLimiter.Redis;

public class RedisRateLimitCounterStore(IConnectionMultiplexer redisConnection) : IRateLimitCounterStore
{
    private readonly IDatabase _redisDatabase = redisConnection.GetDatabase();

    public async Task<RateLimitData?> GetRateLimitDataAsync(string key)
    {
        var data = await _redisDatabase.StringGetAsync(key);
        return data.IsNullOrEmpty ? null : JsonConvert.DeserializeObject<RateLimitData>(data);
    }

    public Task UpdateRateLimitDataAsync(string key, RateLimitData data)
    {
        var serializedData = JsonConvert.SerializeObject(data);
        return _redisDatabase.StringSetAsync(key, serializedData, data.Expiration);
    }
}