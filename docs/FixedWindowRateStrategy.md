## Fixed Window Rate Limiting Strategy

The **Fixed Window** rate limiting strategy controls the number of allowed requests within a specified time window (e.g., 100 requests per minute). Once the window expires, the request count is reset, and a new window begins. This approach provides predictable throttling for APIs or services.

### Example

You can integrate the `FixedWindowRateStrategy` in your application’s rate limiting configuration like so:

```csharp
// Configure a fixed window rate limiting policy
public void ConfigureServices(IServiceCollection services)
{
    services.AddRateLimiter(registry =>
    {
        registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
        {
            options.Window = TimeSpan.FromMinutes(1);   // Time window for rate limiting
            options.PermitLimit = 100;                  // Max number of requests allowed in the window
        });
    });
}
```

### How it Works

- **Window Size**: A fixed period (e.g., 1 minute) during which the system tracks incoming requests.
- **Permit Limit**: The maximum number of requests that can be made during the window. Once this limit is reached, further requests are denied until the window resets.
- **State Storage**: The request count is stored using a backing store like in-memory or Redis to track how many requests are made per window. Distributed systems should use persistent stores like Redis for consistency across servers.

checkout the options available for the `FixedWindowRateStrategy` in the [FixedWindowOptions](FixedWindowOptions.md) documentation.
### When to Use

This strategy is ideal when:
- You want predictable rate limits for regular, repeated traffic.
- You’re limiting user actions in a predictable timeframe (e.g., API calls or account actions).
- The traffic pattern is consistent or easily forecasted.

For services with **bursty traffic**, consider using a burst-capable strategy like **[Token Bucket Strategy](TokenBucketRateStrategy.md)**.