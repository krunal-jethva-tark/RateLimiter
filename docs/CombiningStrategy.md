## Combining Rate Limiting Strategies

In some cases, you might want to combine multiple rate limiting strategies to provide more flexibility and control. For instance, you can apply a **Fixed Window** strategy to control overall traffic and a **Token Bucket** strategy to handle burst traffic more gracefully.

### Example

```csharp
// Combine Fixed Window and Token Bucket Strategies
public void ConfigureServices(IServiceCollection services)
{
    services.AddRateLimiter(registry =>
    {
        // Fixed Window Policy
        registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
        {
            options.Window = TimeSpan.FromMinutes(1);
            options.PermitLimit = 100;
        })
        .MarkAsDefault();

        // Token Bucket Policy for bursty traffic
        registry.AddTokenBucketPolicy("TokenBucketPolicy", options =>
        {
            options.TokenLimit = 100;      // Max burst capacity
            options.RefillRate = 10;       // Tokens refilled per second
            options.PermitLimit = 10;      // Max requests per second
        });
    });
}
```

### How it Works

- **Fixed Window**: Limits the overall number of requests within a given window.
- **Token Bucket**: Allows clients to accumulate unused requests (tokens) and submit bursts of traffic within the specified burst capacity.

By combining these two strategies, you ensure that traffic is controlled both in terms of total volume and short bursts, providing a balanced and flexible rate limiting solution.

### Priority-Based Rate Limiting

Rate limits can also be configured based on the priority of the system or service. For example, higher-priority services can have a larger quota, while lower-priority systems can have a more restrictive limit.s