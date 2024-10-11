using Microsoft.Extensions.DependencyInjection;

namespace RateLimiter;

/// <summary>
/// Provides extension methods for registering the rate limiter services in the Dependency Injection container.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds the rate limiter services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to which the rate limiter services will be added.</param>
    /// <param name="configureOptions">An action to configure the rate limiting policy registry.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the rate limiter services registered.</returns>
    public static IServiceCollection AddRateLimiter(this IServiceCollection services, Action<RateLimiterPolicyRegistry> configureOptions)
    {
        var options = new RateLimiterPolicyRegistry();
        configureOptions(options);

        // Register the rate limiter options as a singleton
        services.AddSingleton(options);

        return services;
    }
}