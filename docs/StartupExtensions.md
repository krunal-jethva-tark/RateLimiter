## Registering Rate Limiter in Dependency Injection

The `RateLimiter` library can be easily integrated with your application using the `AddRateLimiter` extension method. This method registers the rate limiter policies in the Dependency Injection (DI) container, allowing them to be used across the application.

### Example

In your `Startup.cs` or `Program.cs` (depending on your ASP.NET Core version), you can register the rate limiter policies as follows:

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

    // Other service registrations
}
```

### Explanation:

- **IServiceCollection**: The `AddRateLimiter` method extends `IServiceCollection`, enabling you to configure and register rate limiting policies in the DI container.
- **Rate Limiting Policies**: You define and configure rate limiting strategies (such as Fixed Window or Token Bucket) in the `RateLimiterPolicyRegistry` and then register these policies.
- **Singleton Registration**: The `RateLimiterPolicyRegistry` is registered as a singleton, ensuring that the same instance of the registry is used throughout the application.

By adding this registration, your application will be equipped with rate-limiting capabilities across all services that depend on the configured policies.