using Microsoft.AspNetCore.Http;

namespace RateLimiter.KeyGenerators;

/// <summary>
/// Provides methods to generate rate limiting keys based on different criteria.
/// These keys are used to track request limits for users, IP addresses, and services.
/// </summary>
public static class KeyGenerator
{
    /// <summary>
    /// Generates a key based on the authenticated user's identity.
    /// If the user is anonymous, the key will include the IP address.
    /// </summary>
    /// <value>
    /// A function that takes an <see cref="HttpContext"/> and returns a string representing the user-based rate limiting key.
    /// </value>
    public static Func<HttpContext, string> User => context =>
    {
        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "unknown";
        var userId = context.User.Identity?.Name ?? $"anonymous_{ip}";
        return $"rate_limit:user:{userId}";
    };

    /// <summary>
    /// Generates a key based on the client's IP address.
    /// </summary>
    /// <value>
    /// A function that takes an <see cref="HttpContext"/> and returns a string representing the IP-based rate limiting key.
    /// </value>
    public static Func<HttpContext, string> IP => context =>
    {
        var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown-ip";
        return $"rate_limit:ip:{ipAddress}";
    };

    /// <summary>
    /// Generates a key based on the service identifier header.
    /// </summary>
    /// <value>
    /// A function that takes an <see cref="HttpContext"/> and returns a string representing the service-based rate limiting key.
    /// </value>
    public static Func<HttpContext, string> Service => context =>
    {
        var serviceIdentifier = context.Request.Headers["Service-Identifier"].FirstOrDefault() ?? "unknown-service";
        return $"rate_limit:service:{serviceIdentifier}";
    };
}