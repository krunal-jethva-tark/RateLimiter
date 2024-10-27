## Rate Limiting Middleware: `RateLimitingMiddleware`

The `RateLimitingMiddleware` class is designed to implement rate-limiting strategies for incoming HTTP requests in an ASP.NET Core application. This middleware evaluates applicable rate-limiting policies and processes requests according to the specified limits.

### Features

- **Policy-Based Rate Limiting**: Supports both named policies and global policies, allowing for flexible rate-limiting configurations based on application requirements.

- **Automatic Request Rejection**: Automatically rejects requests that exceed the configured rate limits, returning an appropriate HTTP response to the client.

### Example Usage

1. **Register the Middleware**: 
   Ensure that the middleware is registered in your `Startup.cs` or `Program.cs` file (for .NET 6 and later).

   ```csharp
   public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
   {
       app.UseMiddleware<RateLimitingMiddleware>();

       // Additional middleware registrations
       app.UseRouting();
       app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
   }
   ```
   
2. **Register Counter Store**:
   Register a counter store to keep track of request counts and enforce rate limiting policies. This can be done by adding the appropriate implementation of `IRateLimitCounterStore` to the service collection.

   ```csharp
   services.AddSingleton<IRateLimitCounterStore, InMemoryRateLimitCounterStore>();
   ```
3. **Define Rate Limiting Policies**: 
   Register rate-limiting policies within your service configuration.

   ```csharp
   services.AddRateLimiter(registry =>
    {
        registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
        {
            options.Window = TimeSpan.FromMinutes(1);   // Time window for rate limiting
            options.PermitLimit = 100;                  // Max number of requests allowed in the window
        });
    });
   ```

4. **Use Rate Limiting Attributes**: 
   Apply rate-limiting attributes to your controllers or specific action methods.

   ```csharp
   [EnableRateLimiting("MyCustomPolicy")]
   public class MyController : ControllerBase
   {
       public IActionResult Get()
       {
           return Ok("Request is allowed.");
       }
   }
   ```
5. **Disable Rate Limiting for Specific Actions**: 
   You can disable rate limiting for specific actions by applying the `DisableRateLimitingAttribute`.

   ```csharp
   [DisableRateLimiting]
   public IActionResult Get()
   {
       return Ok("Request is allowed without rate limiting.");
   }
   ```

### Handling Limits and Responses

- **Request Limit Exceeded**: When the limit is exceeded, clients receive a **HTTP 429 Too Many Requests** response, indicating that they need to slow down.
- **Rate Limit Headers**: The server may include response headers to notify clients about their remaining requests in the current window. Common headers include:
   - `X-RateLimit-Limit`: The total number of requests allowed in the window.
   - `X-RateLimit-Remaining`: The number of requests remaining.
   - `X-RateLimit-Reset`: Time when the current window resets.

### When to Use

- **Preventing Resource Overload**: This middleware is essential for protecting your application from excessive traffic that can lead to resource exhaustion or degraded performance.

- **API Rate Limiting**: Ideal for APIs that need to enforce usage limits based on user accounts or client IP addresses, ensuring fair use among all clients.

### Prerequisites

- Ensure that at least one rate-limiting policy is defined in your `RateLimiterPolicyRegistry` to utilize this middleware effectively.