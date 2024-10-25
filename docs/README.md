# Rate Limiter Library

## Introduction
This library provides a distributed rate limiting solution, designed to manage and throttle API requests. It supports multiple strategies such as fixed window and token bucket rate limiting, offering flexibility for per-user, per-IP, or per-service rate limiting. The library is optimized for high scalability and performance in distributed environments.

## Features
- Supports **Fixed Window** and **Token Bucket** rate limiting strategies.
- Flexible configuration for **per-user**, **per-IP**, or **per-service** rate limiting.
- **Distributed design** using Redis for scalability.
- Handles burst traffic (in Token Bucket strategy).
- Provides relevant **HTTP headers** to indicate capacity status.

## Documentation

### Rate Limiting Strategies
- [Fixed Window Strategy](./FixedWindowRateStrategy.md)
- [Token Bucket Strategy](./TokenBucketRateStrategy.md)

### Core Components
- [Enable Rate Limiting Attribute](./EnableRateLimitingAttribute.md)
- [Rate Limiting Middleware](./RateLimitingMiddleware.md)
- [Rate Limiter Policy Registry](./RateLimiterPolicyRegistry.md)
- [Rate Limit Counter Store (In-Memory)](./InMemoryRateLimitCounterStore.md)
- [Rate Limit Counter Store (Redis)](./RedisRateLimitCounterStore.md)

### Advanced Topics
- [Rate Limiter Strategy Base](./RateLimiterStrategyBase.md)
- [Rate Limiter Extensions](./RateLimiterExtensions.md)
- [IRateLimitCounterStore Interface](./IRateLimitCounterStore.md)

## Examples
Here is a simple example of how to configure the rate limiter in a .NET application:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRateLimiting(options =>
    {
        options.UseFixedWindowStrategy("fixed", 10, TimeSpan.FromSeconds(1));
        options.UseTokenBucketStrategy("token", 10, 100, TimeSpan.FromSeconds(1));
    });
}
```

## FAQ
### 1. How do I configure Redis for distributed rate limiting?
Ensure that Redis is installed and running on your server. Update the connection strings in the configuration file to point to your Redis instance

### 2. How does burst handling work in the Token Bucket strategy?
The token bucket strategy allows users to accumulate "credits" if they haven’t made requests for a certain period. This allows them to send bursts of requests all at once, up to a certain limit. See [Token Bucket Strategy](./TokenBucketRateStrategy.md) for details.