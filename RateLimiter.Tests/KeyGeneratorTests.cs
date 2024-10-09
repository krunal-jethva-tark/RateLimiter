using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RateLimiter.KeyGenerators;

namespace RateLimiter.Tests;

public class KeyGeneratorTests
{
    [Fact]
    public void UserKey_ShouldReturnCorrectKey_WhenUserIsAuthenticated()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testUser") }))
        };
        context.Request.Headers["X-Forwarded-For"] = "192.168.1.1";

        // Act
        var key = KeyGenerator.User(context);

        // Assert
        Assert.Equal("rate_limit:user:testUser", key);
    }

    [Fact]
    public void UserKey_ShouldReturnAnonymousKey_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()) // Not authenticated
        };
        context.Request.Headers["X-Forwarded-For"] = "192.168.1.1";

        // Act
        var key = KeyGenerator.User(context);

        // Assert
        Assert.Equal("rate_limit:user:anonymous_192.168.1.1", key);
    }

    [Fact]
    public void IPKey_ShouldReturnCorrectKey_WhenXForwardedForHeaderIsPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = "192.168.1.1";

        // Act
        var key = KeyGenerator.IP(context);

        // Assert
        Assert.Equal("rate_limit:ip:192.168.1.1", key);
    }

    [Fact]
    public void IPKey_ShouldReturnCorrectKey_WhenXForwardedForHeaderIsAbsent()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Connection =
            {
                RemoteIpAddress = System.Net.IPAddress.Parse("10.0.0.1")
            }
        };

        // Act
        var key = KeyGenerator.IP(context);

        // Assert
        Assert.Equal("rate_limit:ip:10.0.0.1", key);
    }

    [Fact]
    public void ServiceKey_ShouldReturnCorrectKey_WhenServiceIdentifierHeaderIsPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Service-Identifier"] = "TestService";

        // Act
        var key = KeyGenerator.Service(context);

        // Assert
        Assert.Equal("rate_limit:service:TestService", key);
    }

    [Fact]
    public void ServiceKey_ShouldReturnDefaultKey_WhenServiceIdentifierHeaderIsAbsent()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var key = KeyGenerator.Service(context);

        // Assert
        Assert.Equal("rate_limit:service:unknown-service", key);
    }
}