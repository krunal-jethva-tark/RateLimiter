using Microsoft.AspNetCore.Http;
using Moq;
using RateLimiter.KeyGenerators;
using RateLimiter.Models;
using RateLimiter.Stores;
using RateLimiter.Strategies;

namespace RateLimiter.Tests;

public class FixedWindowRateStrategyTests
{
    private readonly Mock<IRateLimitCounterStore> _counterStoreMock;
    private readonly FixedWindowRateStrategy _strategy;

    public FixedWindowRateStrategyTests()
    {
        _counterStoreMock = new Mock<IRateLimitCounterStore>();
        var options = new FixedWindowOptions
        {
            KeyGenerator = KeyGenerator.User, // Or any other key generator you want to test
            PermitLimit = 5, // Set the maximum requests per second
            Window = TimeSpan.FromSeconds(1) // Set the window duration
        };

        _strategy = new FixedWindowRateStrategy(_counterStoreMock.Object, options);
    }
    
    [Fact]
    public async Task IsRequestPermittedAsync_ShouldAllowRequests_UnderLimit()
    {
        // Arrange
        _counterStoreMock.Setup(m => m.GetRequestCountAsync(It.IsAny<string>())).ReturnsAsync(3);

        // Act
        var result = await _strategy.IsRequestPermittedAsync(CreateHttpContext());

        // Assert
        Assert.True(result);
        _counterStoreMock.Verify(m => m.IncrementRequestCountAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldRejectRequests_OverLimit()
    {
        // Arrange
        _counterStoreMock.Setup(m => m.GetRequestCountAsync(It.IsAny<string>())).ReturnsAsync(5);

        // Act
        var result = await _strategy.IsRequestPermittedAsync(CreateHttpContext());

        // Assert
        Assert.False(result);
        _counterStoreMock.Verify(m => m.IncrementRequestCountAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public async Task IsRequestPermittedAsync_ShouldResetCount_AfterWindow()
    {
        // Arrange
        var key = "test_key";
        _counterStoreMock.Setup(m => m.GetRequestCountAsync(It.IsAny<string>())).ReturnsAsync(5);
         // Simulate that the count resets after the window

        // Act
        var result1 = await _strategy.IsRequestPermittedAsync(CreateHttpContext());
        
        _counterStoreMock.Setup(m => m.GetRequestCountAsync(It.IsAny<string>()))
            .ReturnsAsync(0);
        var result2 = await _strategy.IsRequestPermittedAsync(CreateHttpContext());

        // Assert
        Assert.False(result1); // First request should be denied
        Assert.True(result2); // Second request should be allowed after reset
        _counterStoreMock.Verify(m => m.IncrementRequestCountAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Exactly(1));
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = "192.168.0.1"; // Mock IP for the key
        return context;
    }
}