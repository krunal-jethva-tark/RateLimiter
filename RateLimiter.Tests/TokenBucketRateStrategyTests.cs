using Microsoft.AspNetCore.Http;
using Moq;
using RateLimiter.KeyGenerators;
using RateLimiter.Models;
using RateLimiter.Stores;
using RateLimiter.Strategies;

namespace RateLimiter.Tests;

public class TokenBucketRateStrategyTests
{
    private readonly Mock<IRateLimitCounterStore> _counterStoreMock;
    private readonly TokenBucketRateStrategy _strategy;

    public TokenBucketRateStrategyTests()
    {
        _counterStoreMock = new Mock<IRateLimitCounterStore>();
        var options = new TokenBucketOptions
        {
            KeyGenerator = KeyGenerator.User, // Use your key generator
            MaxRequestsPerSecond = 5, // Maximum tokens per second
            BurstCapacity = 10 // Maximum burst capacity
        };

        _strategy = new TokenBucketRateStrategy(_counterStoreMock.Object, options);
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldAllowRequests_WhenTokensAvailable()
    {
        // Arrange
        var key = "test_key";
        var tokensAvailable = 5; // Initial tokens available
        var lastRefillTime = DateTime.UtcNow.AddSeconds(-1); // Last refill time in the past
        _counterStoreMock.Setup(m => m.GetTokenBucketStatusAsync(key)).ReturnsAsync((tokensAvailable, lastRefillTime));

        // Act
        var result = await _strategy.IsRequestPermittedAsync(CreateHttpContext());

        // Assert
        Assert.True(result);
        _counterStoreMock.Verify(m => m.UpdateTokenBucketAsync(key, tokensAvailable - 1, It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRejectRequests_WhenNoTokensAvailable()
    {
        // Arrange
        var key = "test_key";
        var tokensAvailable = 0; // No tokens available
        var lastRefillTime = DateTime.UtcNow; // Last refill time is now
        _counterStoreMock.Setup(m => m.GetTokenBucketStatusAsync(key)).ReturnsAsync((tokensAvailable, lastRefillTime));

        // Act
        var result = await _strategy.IsRequestPermittedAsync(CreateHttpContext());

        // Assert
        Assert.False(result);
        _counterStoreMock.Verify(m => m.UpdateTokenBucketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRefillTokens_Correctly()
    {
        // Arrange
        var key = "test_key";
        var tokensAvailable = 0; // No tokens available initially
        var lastRefillTime = DateTime.UtcNow.AddSeconds(-2); // Last refill time in the past
        _counterStoreMock.Setup(m => m.GetTokenBucketStatusAsync(key)).ReturnsAsync((tokensAvailable, lastRefillTime));

        // Act
        var result1 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Should allow as tokens will be added
        var result2 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Should allow again as tokens will be added

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        _counterStoreMock.Verify(m => m.UpdateTokenBucketAsync(key, 0, It.IsAny<DateTime>()), Times.Exactly(2)); // Two tokens should be consumed
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldNotExceedBurstCapacity()
    {
        // Arrange
        var key = "test_key";
        var tokensAvailable = 10; // Tokens available at burst capacity
        var lastRefillTime = DateTime.UtcNow.AddSeconds(-1); // Last refill time in the past
        _counterStoreMock.Setup(m => m.GetTokenBucketStatusAsync(key)).ReturnsAsync((tokensAvailable, lastRefillTime));

        // Act
        var result1 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Consume 1 token
        var result2 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Consume 1 token
        var result3 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Consume 1 token
        var result4 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Consume 1 token
        var result5 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Consume 1 token
        var result6 = await _strategy.IsRequestPermittedAsync(CreateHttpContext()); // Should still allow, but burst capacity will not increase

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.True(result3);
        Assert.True(result4);
        Assert.True(result5);
        Assert.False(result6); // This should reject as we exceed burst capacity
        _counterStoreMock.Verify(m => m.UpdateTokenBucketAsync(key, It.IsAny<int>(), It.IsAny<DateTime>()), Times.Exactly(5)); // Five tokens consumed
    }

    private HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = "192.168.0.1"; // Mock IP for the key
        return context;
    }
}