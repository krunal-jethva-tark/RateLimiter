## Registering Rate Limiter in Dependency Injection

The `RateLimiter` library integrates seamlessly into your application through the `AddRateLimiter` extension method. This method allows you to register rate-limiting policies within the Dependency Injection (DI) container, making them accessible throughout your application.

### Example

In your `Startup.cs` or `Program.cs` file (depending on your ASP.NET Core version), you can register the rate limiter policies as follows:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRateLimiter(registry =>
    {
        registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
        {
            options.WindowSize = TimeSpan.FromMinutes(1);
            options.RequestLimit = 100;
        }, isGlobal: true);
    });

    // Additional service registrations
}
```

### Explanation

- **IServiceCollection**: The `AddRateLimiter` method extends the `IServiceCollection` interface, allowing you to configure and register rate-limiting policies directly in the DI container.

- **Rate Limiting Policies**: Within the `RateLimiterPolicyRegistry`, you can define and configure various rate-limiting strategies, such as Fixed Window or Token Bucket, and register them for use in your application.

- **Singleton Registration**: The `RateLimiterPolicyRegistry` is registered as a singleton, ensuring that a single instance of the registry is used throughout the application lifecycle.

By including this registration in your service configuration, your application will be equipped with robust rate-limiting capabilities across all services that depend on the configured policies.

### Additional Considerations

- **Middleware Integration**: After registering the rate limiter, consider integrating the `RateLimitingMiddleware` to apply the policies effectively to incoming requests.

- **Endpoint-Specific Policies**: You can also apply rate-limiting attributes to specific controllers or actions for more granular control over your rate-limiting strategy.