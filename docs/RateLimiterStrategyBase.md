## Creating Custom Rate Limiting Strategies

The `RateLimiterStrategyBase<TOptions>` class serves as a versatile foundation for designing custom rate-limiting strategies. By subclassing this base class, developers can define how their applications manage request limits using various algorithms, including **Fixed Window**, **Token Bucket**, and more.

### Example: Implementing a Fixed Window Strategy

Here is an example of how to implement a fixed window rate-limiting strategy by extending `RateLimiterStrategyBase`:

```csharp
public class FixedWindowRateStrategy : RateLimiterStrategyBase<FixedWindowOptions>
{
    private readonly IRateLimitCounterStore _counterStore;

    public FixedWindowRateStrategy(IRateLimitCounterStore counterStore, FixedWindowOptions options)
        : base(counterStore, options)
    {
        _counterStore = counterStore;
    }

    public override async Task<bool> IsRequestPermittedAsync(string key, DateTime asOfDate)
    {
        var currentCount = await _counterStore.GetRequestCountAsync(key, asOfDate);
        if (currentCount < Options.RequestLimit)
        {
            await _counterStore.IncrementRequestCountAsync(key, asOfDate);
            return true;
        }
        return false;
    }
}
```

### Key Components

- **Options**: The generic type `TOptions` allows you to configure the rate-limiting strategy. For a fixed window strategy, this might include parameters such as the duration of the time window and the maximum number of requests permitted within that window.
  
- **IRateLimitCounterStore**: This interface is utilized to maintain and track the count of requests, enabling the strategy to determine if a client has exceeded the established limits.

### Customizing Rate Limiting Logic

By extending `RateLimiterStrategyBase`, you can implement various rate-limiting algorithms. Some potential strategies include:

- **Sliding Window**: This approach involves tracking the timestamps of requests to maintain a sliding window, thereby allowing requests to be counted continuously over time.

- **Leaky Bucket**: In this strategy, requests "leak" from the bucket at a constant rate, effectively smoothing out bursts of traffic while accommodating temporary surges in usage.

Inheriting from the base class ensures that your custom strategies integrate smoothly with the library’s overall architecture, making them straightforward to manage and apply across your application.