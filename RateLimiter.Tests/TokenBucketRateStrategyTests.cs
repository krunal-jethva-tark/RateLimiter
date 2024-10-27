using Moq;
using RateLimiter.Models;
using RateLimiter.Stores;
using RateLimiter.Strategies;

namespace RateLimiter.Tests;

public class TokenBucketRateStrategyTests
{
    private readonly Mock<IRateLimitCounterStore> _counterStoreMock;
    private readonly TokenBucketOptions _options;
    private readonly TokenBucketRateStrategy _strategy;

    public TokenBucketRateStrategyTests()
    {
        _counterStoreMock = new Mock<IRateLimitCounterStore>();
        _options = new TokenBucketOptions
        {
            MaxRequestsPerSecond = 10, // Example: 10 requests per second
            BurstCapacity = 100 // Example: Up to 100 tokens can be held
        };
        _strategy = new TokenBucketRateStrategy(_counterStoreMock.Object, _options);
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldPermitRequest_WhenTokensAvailable()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 5,
            LastRefillTime = asOfDate.AddSeconds(-1),
            CreatedAt = asOfDate,
        };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));
        
        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, asOfDate);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRejectRequest_WhenNoTokensAvailable()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 0,
            LastRefillTime = asOfDate,
            CreatedAt = asOfDate
        };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));

        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRefillTokenOverTime()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 50, // Initial tokens
            LastRefillTime = asOfDate.AddSeconds(-5), // 5 seconds ago
            CreatedAt = asOfDate
        };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));
        
        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, asOfDate);
        
        // Assert
        Assert.True(result);
        Assert.Equal(asOfDate, rateLimitData.LastRefillTime);
        
        // 10 tokens per second * 5 seconds = 50 tokens added. Burst capacity is 100.
        Assert.Equal(99, rateLimitData.TokensAvailable);
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldHandleBurstTrafficAfterIdlePeriod()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow;
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 0, // No tokens available initially
            LastRefillTime = asOfDate.AddSeconds(-10), // 10 seconds ago
            CreatedAt = asOfDate
        };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));

        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.True(result);
            
        // 10 tokens per second * 10 seconds = 100 tokens added (max burst capacity).
        Assert.Equal(99, rateLimitData.TokensAvailable); // One token consumed for the current request.
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldNotExceedBurstCapacity_WhenRefilling()
    {
        // Arrange
        var key = "test-key";
        var asOfDate = DateTime.UtcNow.AddSeconds(15); // Simulating 15 seconds of idle time
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 80, // Already has 80 tokens
            LastRefillTime = asOfDate.AddSeconds(-15), // Refill last happened 15 seconds ago
            CreatedAt = asOfDate
        };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));

        // Act
        var (result, _) = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.True(result);

        // 10 tokens per second * 15 seconds = 150 tokens added, but burst capacity is capped at 100.
        Assert.Equal(99, rateLimitData.TokensAvailable); // After one token is consumed for the request.
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldAllowMaxBurstRequestsUpToBurstCapacity()
    {
        // Arrange
        const string key = "test-key";
        var asOfDate = DateTime.UtcNow.AddSeconds(10); // Simulating 10 seconds of idle time
        var rateLimitData = new RateLimitData
        {
            TokensAvailable = 0,
            LastRefillTime = asOfDate.AddSeconds(-10), // No activity for 10 seconds
            CreatedAt = asOfDate
        };

        _counterStoreMock.Setup(store => store.GetAndUpdateRateLimitDataAsync(key, asOfDate, It.IsAny<Func<RateLimitData?, DateTime, RateLimitData>>()))
            .ReturnsAsync((string k, DateTime date, Func<RateLimitData?, DateTime, RateLimitData> updateLogic) =>
                updateLogic(rateLimitData, date));

        // Simulate making 100 requests (up to burst capacity)
        for (var i = 0; i < 100; i++)
        {
            var (result, _) = await _strategy.IsRequestPermittedAsync(key, asOfDate);
            Assert.True(result);
        }

        // Act - The 101st request should be rejected
        var (resultAfterBurst, _) = await _strategy.IsRequestPermittedAsync(key, asOfDate);

        // Assert
        Assert.False(resultAfterBurst); // No more tokens available after burst
    }
}