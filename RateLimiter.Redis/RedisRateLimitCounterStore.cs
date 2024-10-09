using RateLimiter.Stores;
using StackExchange.Redis;

namespace RateLimiter.Redis;

public class RedisRateLimitCounterStore: IRateLimitCounterStore
{
    private readonly IDatabase _redisDatabase;

    public RedisRateLimitCounterStore(IConnectionMultiplexer redisConnection)
    {
        _redisDatabase = redisConnection.GetDatabase();
    }

    public async Task<int> GetRequestCountAsync(string key)
    {
        var value = await _redisDatabase.StringGetAsync(key);
        if (value.IsNullOrEmpty) return 0;
        return (int)value;
    }

    public async Task IncrementRequestCountAsync(string key, TimeSpan expiration)
    {
        var transaction = _redisDatabase.CreateTransaction();
        var incrementedValue = await transaction.StringIncrementAsync(key);
        if (incrementedValue == 1)
        {
            await transaction.KeyExpireAsync(key, expiration); // Set expiration if it's the first time
        }
        await transaction.ExecuteAsync();
    }

    public Task ResetRequestCountAsync(string key, TimeSpan expiration)
    {
        return _redisDatabase.StringSetAsync(key, 0, expiration);
    }

    public async Task<(int tokenAvailable, DateTime lastRefillTime)> GetTokenBucketStatusAsync(string key)
    {
        var tokensValue = await _redisDatabase.StringGetAsync($"{key}:tokens");
        var lastRefillValue = await _redisDatabase.StringGetAsync($"{key}:lastRefill");

        int tokensAvailable = tokensValue.IsNullOrEmpty ? 0 : (int)tokensValue;
        DateTime lastRefillTime = lastRefillValue.IsNullOrEmpty ? DateTime.UtcNow : DateTime.Parse(lastRefillValue);

        return (tokensAvailable, lastRefillTime);
    }

    public Task UpdateTokenBucketAsync(string key, int tokensAvailable, DateTime lastRefillTime)
    {
        var lastRefillValue = lastRefillTime.ToString("o"); // ISO 8601 format
        var tasks = new[]
        {
            _redisDatabase.StringSetAsync($"{key}:tokens", tokensAvailable),
            _redisDatabase.StringSetAsync($"{key}:lastRefill", lastRefillValue)
        };
        return Task.WhenAll(tasks);
    }
}