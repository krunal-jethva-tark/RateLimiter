## `TokenBucketOptions`

The `TokenBucketOptions` class is used to configure the token bucket rate limiting strategy. In a token bucket strategy, tokens are added to a bucket at a fixed rate, and each request consumes a token. When the bucket is empty, additional requests are rejected until more tokens are added.

### Parameters

- **MaxRequestsPerSecond**: Specifies the maximum number of requests that can be processed per second.
  - Example: If `MaxRequestsPerSecond` is set to 20, up to 20 requests can be processed per second.
- **BurstCapacity**: Defines the maximum number of requests that can be made in a burst, allowing for sudden spikes in traffic.
  - Example: If `BurstCapacity` is set to 100, up to 100 requests can be made in a burst.

### Example Usage

Here’s how to configure and use the `TokenBucketOptions` in your rate limiting policy:

```csharp
// Create a new instance of TokenBucketOptions
var options = new TokenBucketOptions
{
    MaxRequestsPerSecond = 20,    // Allow 20 requests per second
    BurstCapacity = 100           // Allow bursts of up to 100 requests
};

// Use these options when configuring your rate limiter policy
rateLimiterRegistry.AddTokenBucketPolicy("MyPolicy", opts =>
{
    opts.MaxRequestsPerSecond = options.MaxRequestsPerSecond;
    opts.BurstCapacity = options.BurstCapacity;
});