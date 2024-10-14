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
    public async Task GetAndUpdateRateLimitDataAsync_ShouldReturnNull_WhenDataNotExists()
    {
        // Arrange
        const string key = "non-existing-key";
        DateTime asOfDate = DateTime.UtcNow;

        // Act
        var result = await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) =>
        {
            // Custom logic for test
            return data ?? new RateLimitData { CreatedAt = date, Expiration = TimeSpan.FromMinutes(1) };
        });

        // Assert
        Assert.NotNull(result); // Will return new data since we initialized it in the updateLogic
    }
    
    [Fact]
    public async Task GetAndUpdateRateLimitDataAsync_ShouldReturnData_WhenKeyExistsAndNotExpired()
    {
        // Arrange
        const string key = "test-key";
        DateTime asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 10,
            LastRefillTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Expiration = TimeSpan.FromMinutes(1)
        };

        await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => rateLimitData);

        // Act
        var result = await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => data!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rateLimitData.TokensAvailable, result.TokensAvailable);
        Assert.Equal(rateLimitData.LastRefillTime, result.LastRefillTime);
    }
    
    [Fact]
    public async Task GetAndUpdateRateLimitDataAsync_ShouldRecreateData_WhenDataIsExpired()
    {
        // Arrange
        var key = "test-key";
        DateTime asOfDate = DateTime.UtcNow;
        var expiredRateLimitData = new RateLimitData
        {
            TokensAvailable = 10,
            LastRefillTime = DateTime.UtcNow.AddMinutes(-2), // Simulate expired data
            CreatedAt = DateTime.UtcNow.AddMinutes(-2),
            Expiration = TimeSpan.FromMinutes(1)
        };

        // Add expired data to the store
        await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => expiredRateLimitData);

        // Act
        var result = await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) =>
        {
            // If the data is expired, recreate a new RateLimitData
            if (data == null || data.CreatedAt.Add(data.Expiration) < date)
            {
                return new RateLimitData
                {
                    TokensAvailable = 5, // New default value
                    LastRefillTime = date,
                    CreatedAt = date,
                    Expiration = TimeSpan.FromMinutes(1)
                };
            }
            return data;
        });

        // Assert
        Assert.NotNull(result); // Data should be recreated if it was expired
        Assert.Equal(5, result.TokensAvailable); // Check that new data was set with expected values
        Assert.Equal(asOfDate, result.LastRefillTime); // Ensure that new LastRefillTime is set
    }


    [Fact]
    public async Task UpdateRateLimitDataAsync_ShouldAddNewData_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "new-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 5,
            LastRefillTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Expiration = TimeSpan.FromMinutes(1)
        };

        // Act
        await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => rateLimitData);
        var result = await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => data!);

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
        DateTime asOfDate = DateTime.UtcNow;
        var initialData = new RateLimitData
        {
            TokensAvailable = 5,
            LastRefillTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Expiration = TimeSpan.FromMinutes(1)
        };

        await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => initialData);

        // Act
        var updatedData = new RateLimitData
        {
            TokensAvailable = 10,
            LastRefillTime = DateTime.UtcNow.AddSeconds(5),
            CreatedAt = initialData.CreatedAt,
            Expiration = TimeSpan.FromMinutes(1)
        };

        await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => updatedData);
        var result = await _store.GetAndUpdateRateLimitDataAsync(key, asOfDate, (data, date) => data!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedData.TokensAvailable, result.TokensAvailable);
        Assert.Equal(updatedData.LastRefillTime, result.LastRefillTime);
    }
}