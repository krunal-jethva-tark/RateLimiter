using System.Collections.Concurrent;
using RateLimiter.Models;

namespace RateLimiter.Stores;

public class InMemoryRateLimitCounterStore: IRateLimitCounterStore
{
    private readonly ConcurrentDictionary<string, RateLimitData> _store = new();
    public Task<RateLimitData?> GetRateLimitDataAsync(string key)
    {
        if (_store.TryGetValue(key, out var rateLimitData) && rateLimitData.CreatedAt.Add(rateLimitData.Expiration) > DateTime.UtcNow)
        {
            return Task.FromResult<RateLimitData?>(rateLimitData);
        }
        
        return Task.FromResult<RateLimitData?>(null);
    }

    public Task UpdateRateLimitDataAsync(string key, RateLimitData data)
    {
        _store.AddOrUpdate(key, data, (existingKey, existingData) =>
        {
            // Update with new data
            existingData.Count = data.Count;
            existingData.TokensAvailable = data.TokensAvailable;
            existingData.LastRefillTime = data.LastRefillTime;
            existingData.Expiration = data.Expiration;
            return existingData;
        });

        return Task.CompletedTask;
    }
}