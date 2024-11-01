## Rate Limiter Library

### Introduction
This library provides a distributed rate limiting solution, designed to manage and throttle API requests. It supports multiple strategies such as fixed window and token bucket rate limiting, offering flexibility for per-user, per-IP, or per-service rate limiting. The library is optimized for high scalability and performance in distributed environments.

### Features
- Supports **Fixed Window** and **Token Bucket** rate limiting strategies.
- Flexible configuration for **per-user**, **per-IP**, or **per-service** rate limiting.
- **Distributed design** using Redis for scalability.
- Handles burst traffic (in Token Bucket strategy).
- Provides relevant **HTTP headers** to indicate capacity status.

### Documentation

#### Rate Limiting Strategies
- [Fixed Window Strategy](/RateLimiter/FixedWindowRateStrategy.md)
- [Token Bucket Strategy](/RateLimiter/TokenBucketRateStrategy.md)

#### Core Components

- [Enable Rate Limiting Attribute](/RateLimiter/EnableRateLimitingAttribute.md)
- [Disable Rate Limiting Attribute](/RateLimiter/DisableRateLimitingAttribute.md)
- [Rate Limiting Middleware](/RateLimiter/RateLimitingMiddleware.md)

#### Advanced Topics
- [Rate Limiter Strategy Base](/RateLimiter/RateLimiterStrategyBase.md)
- [Rate Limiter Extensions](/RateLimiter/RateLimiterExtensions.md)
- [IRateLimitCounterStore Interface](/RateLimiter/IRateLimitCounterStore.md)
- [Rate Limiter Policy Registry](/RateLimiter/RateLimiterPolicyRegistry.md)

### Example
Here is a simple example of how to configure the rate limiter in a .NET application:

```csharp
builder.Services.AddSingleton<IRateLimitCounterStore, InMemoryRateLimitCounterStore>();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowPolicy("fixed", fixedWindowOptions =>
    {
        fixedWindowOptions.PermitLimit = 20;
        fixedWindowOptions.Window = TimeSpan.FromSeconds(1);
        fixedWindowOptions.KeyGenerator = context => context.Request.Headers["User-Identity"].ToString() ??  $"anonymous";
    })
    .MarkAsDefault();

    options.AddTokenBucketPolicy("token", tokenBucketOptions =>
    {
        tokenBucketOptions.MaxRequestsPerSecond = 20;
        tokenBucketOptions.BurstCapacity = 100;
    })
});

var app = builder.Build();

app.UseMiddleware<RateLimitingMiddleware>()
```