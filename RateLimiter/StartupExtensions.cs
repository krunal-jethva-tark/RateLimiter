using Microsoft.Extensions.DependencyInjection;

namespace RateLimiter;

/// <summary>
/// Provides extension methods for integrating the rate limiter services
/// into the application's Dependency Injection (DI) container.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Registers the rate limiter services in the Dependency Injection (DI) container.
    /// This extension method allows configuring rate limiting policies and registering them
    /// for use throughout the application.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> where rate limiter services will be registered.
    /// This collection is used to configure and manage the application's service dependencies.
    /// </param>
    /// <param name="configureOptions">
    /// An action that configures the <see cref="RateLimiterPolicyRegistry"/>, allowing you to define
    /// different rate limiting policies for your application. 
    /// This could include policies like Fixed Window or Token Bucket strategies.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with the rate limiter services registered.
    /// This allows chaining of additional service registrations.
    /// </returns>
    /// <example>
    /// Example usage:
    /// <code>
    /// services.AddRateLimiter(registry =>
    /// {
    ///     registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
    ///     {
    ///         options.WindowSize = TimeSpan.FromMinutes(1);
    ///         options.RequestLimit = 100;
    ///     }, isGlobal: true);
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddRateLimiter(this IServiceCollection services, Action<RateLimiterPolicyRegistry> configureOptions)
    {
        var options = new RateLimiterPolicyRegistry();
        configureOptions(options);

        // Register the rate limiter options as a singleton
        services.AddSingleton(options);

        return services;
    }
}