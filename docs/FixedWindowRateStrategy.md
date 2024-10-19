# Fixed Window Rate Limiting Strategy

The **Fixed Window** rate limiting strategy controls the number of allowed requests within a specified time window (e.g., 100 requests per minute). Once the window expires, the request count is reset, and a new window begins. This approach provides predictable throttling for APIs or services.

## Example: How to Use `FixedWindowRateStrategy`

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

### Handling Limits and Responses

- **Request Limit Exceeded**: When the limit is exceeded, clients receive a **HTTP 429 Too Many Requests** response, indicating that they need to slow down.
- **Rate Limit Headers**: The server may include response headers to notify clients about their remaining requests in the current window. Common headers include:
  - `X-RateLimit-Limit`: The total number of requests allowed in the window.
  - `X-RateLimit-Remaining`: The number of requests remaining.
  - `X-RateLimit-Reset`: Time when the current window resets.

## When to Use

This strategy is ideal when:
- You want predictable rate limits for regular, repeated traffic.
- You’re limiting user actions in a predictable timeframe (e.g., API calls or account actions).
- The traffic pattern is consistent or easily forecasted.

For services with **bursty traffic**, consider using a burst-capable strategy like **Token Bucket**.

---

## Conclusion

The `FixedWindowRateStrategy` is a simple yet effective approach to controlling traffic with predictable limits. By combining it with other strategies, like the **Token Bucket** approach, or applying it on a per-user, per-IP, or per-service basis, you can tailor rate limiting to meet your specific requirements.

This flexibility ensures your API is protected from abuse while still offering optimal performance for legitimate users.