using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Stores;
using StackExchange.Redis;

namespace RateLimiter.Redis;

/// <summary>
/// Extension methods for setting up Redis rate limiting in an <see cref="IServiceCollection"/>.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Adds Redis-based rate limiting services to the DI container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">An optional action to configure the rate limiter policy registry.</param>
    /// <param name="redisConfiguration">The Redis configuration string for connecting to Redis.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRedisRateLimiting(this IServiceCollection services,  string redisConfiguration)
    {
        var redisConnection = ConnectionMultiplexer.Connect(redisConfiguration);
        services.AddSingleton<IConnectionMultiplexer>(redisConnection);
        services.AddSingleton<IRateLimitCounterStore, RedisRateLimitCounterStore>();
        return services;
    }
}