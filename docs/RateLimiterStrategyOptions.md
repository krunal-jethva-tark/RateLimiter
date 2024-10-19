## Rate Limiter Strategy Options: `RateLimiterStrategyOptions`

The `RateLimiterStrategyOptions` class acts as a foundational configuration class for various rate-limiting strategies. It provides essential settings that dictate how requests are handled and rejected when they exceed defined limits.

### Features

- **Key Generator**: This feature enables customization of how unique keys are generated for rate limiting based on the request context of the client. This allows for flexible and precise rate limiting.

- **Rejection Status Code**: This configurable HTTP status code is returned when a request surpasses the defined rate limit, informing the client about the rejection.

- **Rejection Message**: A customizable response message sent to clients when their requests are denied due to rate limiting. This can enhance client communication and understanding of rate limits.

### Example Usage

```csharp
// Create an instance of RateLimiterStrategyOptions
var options = new RateLimiterStrategyOptions
{
    KeyGenerator = context => context.Connection.RemoteIpAddress?.ToString() ?? "unknown", // Custom key based on IP address
    RejectionStatusCode = StatusCodes.Status429TooManyRequests, // Default status code for too many requests
    RejectionMessage = "You have exceeded the number of allowed requests. Please wait before retrying." // Custom message for rejected requests
};

// Use this options instance when configuring your rate limiting strategy
rateLimiterPolicyRegistry.AddFixedWindowPolicy("MyRateLimitPolicy", opts =>
{
    opts.PermitLimit = 10; // Maximum of 10 requests allowed
    opts.Window = TimeSpan.FromMinutes(1); // Time window of 1 minute
});
```

### When to Use

- **Customizing Rate Limits**: Utilize this options class to tailor the rate-limiting behavior according to your application's specific needs.

- **Client Feedback**: Adjust the rejection status code and message to provide clearer communication to clients regarding their request limits, helping them understand the constraints they face.

### Prerequisites

- This class is typically employed in conjunction with specific rate-limiting strategies, such as Fixed Window or Token Bucket, to effectively manage and enforce request limits.