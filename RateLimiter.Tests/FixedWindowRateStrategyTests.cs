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
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData { Count = 2, Expiration = _options.Window, CreatedAt = asOfDate };

        _counterStoreMock.Setup(x => x.GetRateLimitDataAsync(key))
            .ReturnsAsync(rateLimitData);

        // Act
        var result = await _strategy.IsRequestPermittedAsync(key, _asOfDate);

        // Assert
        Assert.True(result); // Request is permitted since it's within the limit
        _counterStoreMock.Verify(x => x.UpdateRateLimitDataAsync(key, It.IsAny<RateLimitData>()), Times.Once);
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRejectRequests_WhenRateLimitExceeded()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData { Count = 5, Expiration = _options.Window, CreatedAt = asOfDate };

        _counterStoreMock.Setup(store => store.GetRateLimitDataAsync(key))
            .ReturnsAsync(rateLimitData);

        // Act
        var result = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.False(result);
        _counterStoreMock.Verify(store => store.UpdateRateLimitDataAsync(key, It.IsAny<RateLimitData>()), Times.Never);
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldInitializeRateLimitData_IfNotExists()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;

        _counterStoreMock.Setup(store => store.GetRateLimitDataAsync(key))
            .ReturnsAsync((RateLimitData)null);

        // Act
        var result = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.True(result);
        _counterStoreMock.Verify(store => store.UpdateRateLimitDataAsync(key, It.IsAny<RateLimitData>()), Times.Once);
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldHandleMultipleRequests_WithinLimit()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData { Count = 0, Expiration = _options.Window, CreatedAt = asOfDate };

        _counterStoreMock.Setup(store => store.GetRateLimitDataAsync(key))
            .ReturnsAsync(rateLimitData);

        // Simulate 5 requests within the rate limit
        for (var i = 0; i < 5; i++)
        {
            var result = await _strategy.IsRequestPermittedAsync(key, asOfDate);
            Assert.True(result);
        }

        // Act - 6th request should be rejected
        var exceededResult = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.False(exceededResult);
        _counterStoreMock.Verify(store => store.UpdateRateLimitDataAsync(key, It.IsAny<RateLimitData>()), Times.Exactly(5));
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRejectBurstTraffic_ExceedingLimit()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData { Count = 4, Expiration = _options.Window, CreatedAt = asOfDate };

        _counterStoreMock.Setup(store => store.GetRateLimitDataAsync(key))
            .ReturnsAsync(rateLimitData);

        // Act - 1 more request within limit
        var resultWithinLimit = await _strategy.IsRequestPermittedAsync(key, asOfDate);
        var resultExceedingLimit = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.True(resultWithinLimit); // 5th request within limit should succeed
        Assert.False(resultExceedingLimit); // 6th request should be rejected
    }
}