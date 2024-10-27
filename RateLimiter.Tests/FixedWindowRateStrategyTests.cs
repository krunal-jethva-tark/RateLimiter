using Moq;
using RateLimiter.Models;
using RateLimiter.Stores;
using RateLimiter.Strategies;

namespace RateLimiter.Tests;

public class FixedWindowRateStrategyTests
{
    private readonly Mock<IRateLimitCounterStore> _counterStoreMock;
    private readonly FixedWindowRateStrategy _strategy;
    private readonly FixedWindowOptions _options;
    private readonly DateTime _asOfDate;

    public FixedWindowRateStrategyTests()
    {
        _counterStoreMock = new Mock<IRateLimitCounterStore>();

        // Set the rate limit to allow 5 requests per second
        _options = new FixedWindowOptions
        {
            PermitLimit = 5, // Allow 5 requests per window
            Window = TimeSpan.FromSeconds(1) // 1-second window
        };

        _asOfDate = DateTime.UtcNow;

        _strategy = new FixedWindowRateStrategy(_counterStoreMock.Object, _options);
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldPermitRequests_WithinRateLimit()
    {
        // Arrange
        const string key = "test-key";
        var rateLimitData = new RateLimitData { Count = 2, Expiration = _options.Window, CreatedAt = _asOfDate };

        _counterStoreMock.Setup(x => x.GetAndUpdateRateLimitDataAsync(key, _asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) => 
                updateLogic(rateLimitData, date));

        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, _asOfDate);

        // Assert
        Assert.True(result); // Request is permitted since it's within the limit
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRejectRequests_WhenRateLimitExceeded()
    {
        // Arrange
        const string key = "test-key";
        var rateLimitData = new RateLimitData { Count = 5, Expiration = _options.Window, CreatedAt = _asOfDate };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, _asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));

        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, _asOfDate);

        // Assert
        Assert.False(result); // Request should be rejected since the rate limit has been exceeded
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldInitializeRateLimitData_IfNotExists()
    {
        // Arrange
        const string key = "test-key";
        RateLimitData? nullRateLimitData = null; // Simulate no existing data

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, _asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(nullRateLimitData, date)); // Null data should trigger initialization

        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, _asOfDate);

        // Assert
        Assert.True(result); // Request is permitted because new data should be initialized
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldHandleMultipleRequests_WithinLimit()
    {
        // Arrange
        const string key = "test-key";
        var rateLimitData = new RateLimitData { Count = 0, Expiration = _options.Window, CreatedAt = _asOfDate };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, _asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));

        // Act and Assert for 5 requests within the rate limit
        for (var i = 0; i < 5; i++)
        {
            var (result, _) = await _strategy.IsRequestPermittedAsync(key, _asOfDate);
            Assert.True(result); // All should be permitted within the limit
        }

        // Act - 6th request should be rejected
        var (exceededResult, _) = await _strategy.IsRequestPermittedAsync(key, _asOfDate);

        // Assert
        Assert.False(exceededResult); // 6th request exceeds the limit and should be rejected
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRejectBurstTraffic_ExceedingLimit()
    {
        // Arrange
        const string key = "test-key";
        var rateLimitData = new RateLimitData { Count = 4, Expiration = _options.Window, CreatedAt = _asOfDate };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, _asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));

        // Act - 5th request should be within the limit
        var (resultWithinLimit, _) = await _strategy.IsRequestPermittedAsync(key, _asOfDate);

        // 6th request exceeds the limit
        var (resultExceedingLimit, _) = await _strategy.IsRequestPermittedAsync(key, _asOfDate);

        // Assert
        Assert.True(resultWithinLimit); // 5th request within limit should succeed
        Assert.False(resultExceedingLimit); // 6th request should be rejected as it exceeds the limit
    }
}