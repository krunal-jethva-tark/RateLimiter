## Rate Limiter Extension Methods
### Overview
The `RateLimiterExtensions` class provides extension methods to simplify the registration of rate-limiting policies for an application. This document outlines the usage of two main types of rate-limiting strategies: **Fixed Window** and **Token Bucket**. These methods allow developers to easily add rate-limiting policies to a policy registry for use in their applications, including options to configure the policies and define whether they should be applied globally.

### AddFixedWindowPolicy

Registers a Fixed Window rate limiting policy in the provided policy registry. The Fixed Window strategy limits the number of requests during a defined time window. The window resets after each interval, and the request count is reset to zero.

- **Parameters**:
   - `registry` (`RateLimiterPolicyRegistry`): The `RateLimiterPolicyRegistry` where the rate limiting policy will be registered.
   - `policyName` (`string`): A string that uniquely identifies the rate limiting policy.
   - `configure` (`Action<FixedWindowOptions>`): A delegate used to configure the options for the Fixed Window policy, including window size and request limits.

- **Example**:
  ```csharp
  registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
  {
      options.WindowSize = TimeSpan.FromMinutes(1);
      options.RequestLimit = 100;
  });

### AddTokenBucketPolicy

Registers a Token Bucket rate limiting policy in the provided policy registry. The Token Bucket strategy controls request rates by allowing requests until the token bucket is empty. Tokens are refilled at a regular rate, allowing for burst traffic handling.

- **Parameters**:
   - `registry` (`RateLimiterPolicyRegistry`): The `RateLimiterPolicyRegistry` where the rate limiting policy will be registered.
   - `policyName` (`string`): A string that uniquely identifies the rate limiting policy.
   - `configure` (`Action<TokenBucketOptions>`): A delegate used to configure the options for the Token Bucket policy, such as token limit and refill rate.

- **Example**:
  ```csharp
  registry.AddTokenBucketPolicy("TokenBucketPolicy", tokenBucketOptions =>
  {
      tokenBucketOptions.MaxRequestsPerSecond = 20;
      tokenBucketOptions.BurstCapacity = 100;
  })
  ```

### MarkAsDefault
~~~~
Marks the last registered rate limiting policy as the default policy. This method sets the default policy for the rate limiter, which will be used if no specific policy is specified.

- **Parameters**:
   - `registry` (`RateLimiterPolicyRegistry`): The `RateLimiterPolicyRegistry` where the rate limiting policy will be registered.

- **Example**:
  ```csharp
  registry.AddTokenBucketPolicy("DefaultStretegy", tokenBucketOptions =>
  {
      tokenBucketOptions.MaxRequestsPerSecond = 20;
      tokenBucketOptions.BurstCapacity = 100;
  })
  .MarkAsDefault();
  ```
