using System.Collections.Concurrent;

namespace RateLimiter.Stores;

public class InMemoryRateLimitCounterStore: IRateLimitCounterStore
{
    private class CounterData
    {
        public int Count { get; set; }
        public DateTime Expiration { get; set; }
    }
    
    private class TokenBucketData
    {
        public int TokensAvailable { get; set; }
        public DateTime LastRefillTime { get; set; }
    }

    private readonly ConcurrentDictionary<string, CounterData> _counters = new();
    private readonly ConcurrentDictionary<string, TokenBucketData> _tokenBuckets = new();
    
    public Task<int> GetRequestCountAsync(string key)
    {
        if (_counters.TryGetValue(key, out var counter) && counter.Expiration > DateTime.UtcNow)
        {
            return Task.FromResult(counter.Count);
        }

        return Task.FromResult(0);
    }

    public Task IncrementRequestCountAsync(string key, TimeSpan expiration)
    {
        _counters.AddOrUpdate(key, new CounterData
        {
            Count = 1,
            Expiration = DateTime.UtcNow.Add(expiration) // Set expiration for the window
        }, (existingKey, existingCounter) =>
        {
            if (existingCounter.Expiration < DateTime.UtcNow)
            {
                existingCounter.Count = 1;
                existingCounter.Expiration = DateTime.UtcNow.Add(expiration);
            }
            else
            {
                existingCounter.Count++;
            }

            return existingCounter;
        });

        return Task.CompletedTask;
    }

    public Task ResetRequestCountAsync(string key, TimeSpan expiration)
    {
        _counters[key] = new CounterData
        {
            Count = 0,
            Expiration = DateTime.UtcNow.Add(expiration)
        };
        return Task.CompletedTask;
    }

    public Task<(int tokenAvailable, DateTime lastRefillTime)> GetTokenBucketStatusAsync(string key)
    {
        // Get the token bucket status
        if (_tokenBuckets.TryGetValue(key, out var bucketData))
        {
            return Task.FromResult((bucketData.TokensAvailable, bucketData.LastRefillTime));
        }

        // If no bucket exists, return default values (0 tokens and current time)
        return Task.FromResult((0, DateTime.UtcNow));
    }

    public Task UpdateTokenBucketAsync(string key, int tokensAvailable, DateTime lastRefillTime)
    {
        _tokenBuckets[key] = new TokenBucketData
        {
            TokensAvailable = tokensAvailable,
            LastRefillTime = lastRefillTime
        };
        return Task.CompletedTask;
    }

    // Optional: do we need to cleanup dictionary time to time.
    private void CleanUpExpiredEntries()
    {
        var now = DateTime.UtcNow;
        foreach (var key in _counters.Keys)
        {
            if (_counters.TryGetValue(key, out var counter) && counter.Expiration <= now)
            {
                _counters.TryRemove(key, out _); // Remove expired entries
            }
        }
    }
}