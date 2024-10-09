using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Models;

namespace RateLimiter.Redis;

public static class StartupExtensions
{
    public static IServiceCollection AddTarkRedisRateLimiting(this IServiceCollection services, Action<RateLimiterPolicyRegistry> configure)
    {
        // services.AddTarkRateLimitingStores<RedisRateLimitCounterStore>(configure);
        // TODO: add redis store in service
        return services;
    }
}