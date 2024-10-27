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
    return strategy;
});
```

### Example: Retrieve and Apply a Policy

After registering a policy, you can retrieve and apply it to incoming requests as follows:

```csharp
var strategyFactory = registry.GetPolicy("FixedWindowPolicy");
if (strategyFactory != null)
{
    var strategy = strategyFactory(context);
    // Apply the strategy to the current request
}
```

### Methods

#### Register Policy

Registers a new rate-limiting policy in the registry. The policy is associated with a name and a strategy factory. If a policy with the same name already exists, an `InvalidOperationException` is thrown.

- **Parameters**:
    - `policyName` (string): A string that uniquely identifies the rate-limiting policy. This name is used to reference the policy when applying it to different API endpoints or parts of the application.
    - `strategyFactory` (Func<HttpContext, RateLimiterStrategyBase<RateLimiterStrategyOptions>>): A factory function that creates an instance of `RateLimiterStrategyBase<TOptions>` representing the rate limiter strategy for the specified policy. The strategy defines how requests are throttled or allowed based on policy settings.

- **Example**:
  ```csharp
  registry.RegisterPolicy("MyFixedWindowPolicy", context =>
  {
      var counterStore = (IRateLimitCounterStore)context.RequestServices.GetService(typeof(IRateLimitCounterStore))!;
      var strategy = new FixedWindowRateStrategy(counterStore, new FixedWindowOptions());
      return strategy;
  });
  ```

#### Get Policy

Retrieves the rate-limiting policy associated with the given policy name.

- **Parameters**:
    - `policyName` (string): The name of the policy to retrieve.

- **Returns**:
    - `Func<HttpContext, RateLimiterStrategyBase<RateLimiterStrategyOptions>>?`: A factory function that creates an instance of `RateLimiterStrategyBase<TOptions>` representing the rate limiter strategy for the specified policy. Returns `null` if no policy with the specified name exists.

- **Example**:
  ```csharp
  var strategy = registry.GetPolicy("MyPolicy");
  if (strategy != null)
  {
      // Apply the strategy
  }
  ```

#### Set Default Policy

Marks the last registered rate-limiting policy as the default policy. This method sets the default policy for the rate limiter, which will be used if no specific policy is specified.

- **Exceptions**:
    - `InvalidOperationException`: Thrown if no policy has been registered yet.

- **Example**:
  ```csharp
  registry.SetDefaultPolicy();
  ```


### Additional Notes
- **Flexibility**: The `RateLimiterPolicyRegistry` allows for a modular approach to rate limiting. You can easily add, remove, or modify policies as application needs change.
- **Contextual Strategies**: Policies can utilize different strategies based on the context, making the system adaptable to varying load conditions or user behaviors.