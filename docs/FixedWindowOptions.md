# Fixed Window Options: `FixedWindowOptions`

The `FixedWindowOptions` class is used to configure the fixed window rate limiting strategy. In a fixed window strategy, requests are counted within a defined time frame, and once the limit is exceeded within that time window, additional requests are rejected until the next window starts.

## Parameters

- **PermitLimit**: Specifies the maximum number of requests allowed during a time window.
  - Example: If `PermitLimit` is set to 100, only 100 requests will be allowed within the window.
- **Window**: Defines the length of the time window during which requests are counted.
  - Example: Setting `Window` to `TimeSpan.FromMinutes(1)` counts requests over a 1-minute period.

## Example Usage

Here’s how to configure and use the `FixedWindowOptions` in your rate limiting policy:

```csharp
// Create a new instance of FixedWindowOptions
var options = new FixedWindowOptions
{
    PermitLimit = 100,    // Allow 100 requests
    Window = TimeSpan.FromMinutes(1) // Time window of 1 minute
};

// Use these options when configuring your rate limiter policy
rateLimiterRegistry.AddFixedWindowPolicy("MyPolicy", opts =>
{
    opts.PermitLimit = options.PermitLimit;
    opts.Window = options.Window;
});
