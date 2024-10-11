using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Stores;

namespace RateLimiter.Redis;

public static class StartupExtensions
{
    public static IServiceCollection AddRedisRateLimiting(this IServiceCollection services, Action<RateLimiterPolicyRegistry> configure)
    {
        services.AddSingleton<IRateLimitCounterStore, RedisRateLimitCounterStore>();
        return services;
    }
}