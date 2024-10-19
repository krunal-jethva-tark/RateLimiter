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

2. **Define Rate Limiting Policies**: 
   Register rate-limiting policies within your service configuration.

   ```csharp
   services.AddSingleton<RateLimiterPolicyRegistry>(serviceProvider =>
   {
       var registry = new RateLimiterPolicyRegistry();
       // Register your policies here
       return registry;
   });
   ```

3. **Use Rate Limiting Attributes**: 
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

### When to Use

- **Preventing Resource Overload**: This middleware is essential for protecting your application from excessive traffic that can lead to resource exhaustion or degraded performance.

- **API Rate Limiting**: Ideal for APIs that need to enforce usage limits based on user accounts or client IP addresses, ensuring fair use among all clients.

### Prerequisites

- Ensure that at least one rate-limiting policy is defined in your `RateLimiterPolicyRegistry` to utilize this middleware effectively.