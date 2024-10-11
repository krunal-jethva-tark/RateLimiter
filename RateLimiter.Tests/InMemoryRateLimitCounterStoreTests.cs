using RateLimiter.Models;
using RateLimiter.Stores;

namespace RateLimiter.Tests;

public class InMemoryRateLimitCounterStoreTests
{
    private readonly InMemoryRateLimitCounterStore _store;

    public InMemoryRateLimitCounterStoreTests()
    {
        _store = new InMemoryRateLimitCounterStore();
    }
    
    [Fact]
    public async Task GetRateLimitDataAsync_ShouldReturnNull_WhenDataNotExists()
    {
        // Arrange
        const string key = "non-existing-key";

        // Act
        var result = await _store.GetRateLimitDataAsync(key);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetRateLimitDataAsync_ShouldReturnData_WhenKeyExistsAndNotExpired()
    {
        // Arrange
        const string key = "test-key";
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 10,
            LastRefillTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Expiration = TimeSpan.FromMinutes(1)
        };

        await _store.UpdateRateLimitDataAsync(key, rateLimitData);

        // Act
        var result = await _store.GetRateLimitDataAsync(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rateLimitData.TokensAvailable, result.TokensAvailable);
        Assert.Equal(rateLimitData.LastRefillTime, result.LastRefillTime);
    }
    
    [Fact]
    public async Task GetRateLimitDataAsync_ShouldReturnNull_WhenDataIsExpired()
    {
        // Arrange
        var key = "test-key";
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 10,
            LastRefillTime = DateTime.UtcNow.AddMinutes(-2), // Simulate expired data
            CreatedAt = DateTime.UtcNow.AddMinutes(-2),
            Expiration = TimeSpan.FromMinutes(1)
        };

        await _store.UpdateRateLimitDataAsync(key, rateLimitData);

        // Act
        var result = await _store.GetRateLimitDataAsync(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateRateLimitDataAsync_ShouldAddNewData_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "new-key";
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 5,
            LastRefillTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Expiration = TimeSpan.FromMinutes(1)
        };

        // Act
        await _store.UpdateRateLimitDataAsync(key, rateLimitData);

        var result = await _store.GetRateLimitDataAsync(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rateLimitData.TokensAvailable, result.TokensAvailable);
        Assert.Equal(rateLimitData.LastRefillTime, result.LastRefillTime);
    }

    [Fact]
    public async Task UpdateRateLimitDataAsync_ShouldUpdateExistingData_WhenKeyExists()
    {
        // Arrange
        var key = "existing-key";
        var initialData = new RateLimitData
        {
            TokensAvailable = 5,
            LastRefillTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Expiration = TimeSpan.FromMinutes(1)
        };

        await _store.UpdateRateLimitDataAsync(key, initialData);

        // Act
        var updatedData = new RateLimitData
        {
            TokensAvailable = 10,
            LastRefillTime = DateTime.UtcNow.AddSeconds(5),
            CreatedAt = initialData.CreatedAt,
            Expiration = TimeSpan.FromMinutes(1)
        };

        await _store.UpdateRateLimitDataAsync(key, updatedData);
        var result = await _store.GetRateLimitDataAsync(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedData.TokensAvailable, result.TokensAvailable);
        Assert.Equal(updatedData.LastRefillTime, result.LastRefillTime);
    }
}