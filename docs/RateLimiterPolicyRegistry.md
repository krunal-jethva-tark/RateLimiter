### README Section Update for `RateLimiterPolicyRegistry`

## RateLimiterPolicyRegistry

The `RateLimiterPolicyRegistry` is a central component of the rate limiter library that manages and stores rate-limiting policies. These policies define how requests are throttled or allowed based on different strategies (e.g., **Fixed Window**, **Token Bucket**). Each policy is identified by a unique name, making it easy to apply different rate-limiting rules to various parts of an application.

### Features
- **Policy Registration**: Register new rate-limiting policies with a unique name for easy identification and management.
- **Retrieve Policies**: Access previously registered policies to handle incoming requests efficiently.
- **Global Rate Limiting**: Policies can be flagged as global, meaning they apply across all API endpoints in your application, simplifying configuration and maintenance.

### Example: Register a Fixed Window Policy

This example demonstrates how to register a fixed window rate-limiting policy:

```csharp
var registry = new RateLimiterPolicyRegistry();
registry.RegisterPolicy("FixedWindowPolicy", context =>
{
    var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
    var strategy = new FixedWindowRateStrategy(counterStore, new FixedWindowOptions
    {
        WindowSize = TimeSpan.FromMinutes(1), // Set the duration of the rate limiting window
        RequestLimit = 100                     // Set the maximum number of requests allowed in the window
    });
    return (strategy, isGlobal: true); // Flag the policy as global
});
```

### Example: Retrieve and Apply a Policy

After registering a policy, you can retrieve and apply it to incoming requests as follows:

```csharp
var strategyFactory = registry.GetPolicy("FixedWindowPolicy");
if (strategyFactory != null)
{
    var (strategy, isGlobal) = strategyFactory(context);
    // Apply the strategy to the current request
}
```

### Method Descriptions

- **RegisterPolicy()**: Registers a rate-limiting policy with a factory method that defines how the rate-limiting strategy should be created based on the current request context.
- **GetPolicy()**: Retrieves a policy by its unique name and provides a strategy to apply for the current HTTP request.

### Additional Notes
- **Flexibility**: The `RateLimiterPolicyRegistry` allows for a modular approach to rate limiting. You can easily add, remove, or modify policies as application needs change.
- **Contextual Strategies**: Policies can utilize different strategies based on the context, making the system adaptable to varying load conditions or user behaviors.