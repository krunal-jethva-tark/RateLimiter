## Token Bucket Rate Limiting Strategy

The **Token Bucket** rate limiting strategy is designed to accommodate bursts of requests while ensuring a controlled and consistent request rate over time. This strategy is particularly useful when you want to allow short surges in traffic while maintaining an overall limit on requests.

### Example: How to Use `TokenBucketRateStrategy`

To integrate the `TokenBucketRateStrategy` into your rate limiting configuration, you can follow the example below:

```csharp
// Configure a token bucket rate limiting policy
public void ConfigureServices(IServiceCollection services)
{
    services.AddRateLimiter(registry =>
    {
        registry.AddTokenBucketPolicy("TokenBucketPolicy", options =>
        {
            options.BurstCapacity = 50;                   // Maximum tokens available for bursts
            options.MaxRequestsPerSecond = 5;             // Refill rate (tokens added per second)
        });
    });
}
```

### How It Works

- **Burst Capacity**: This setting defines the maximum number of tokens that can be available at any given time, allowing your application to handle bursts of up to this number of requests. Once tokens are consumed, additional requests will either be delayed or denied until more tokens are available.

- **Token Refill Rate**: Tokens are replenished at a specified rate. For instance, if the refill rate is set to 5 tokens per second, then after one second, 5 new tokens will be available for consumption. This enables a steady and manageable rate of requests over time.

- **State Storage**: The state of the token bucket, including the number of tokens available and the last time tokens were refilled, must be stored in a backing store. This could be an in-memory store or a distributed cache, depending on your application's architecture.

### Use Cases

The Token Bucket strategy is particularly effective in scenarios where:

- You expect occasional bursts of high traffic and need to allow for quick processing of these requests.
- You require long-term throttling of requests to prevent abuse or overloading of your service.

By configuring the Token Bucket strategy appropriately, you can strike a balance between accommodating peak traffic and maintaining a controlled overall request rate.