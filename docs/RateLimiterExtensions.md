### Rate Limiter Extension Methods
#### Overview
The `RateLimiterExtensions` class provides extension methods to simplify the registration of rate-limiting policies for an application. This document outlines the usage of two main types of rate-limiting strategies: **Fixed Window** and **Token Bucket**. These methods allow developers to easily add rate-limiting policies to a policy registry for use in their applications, including options to configure the policies and define whether they should be applied globally.

---

### Table of Contents
1. [Fixed Window Rate Limiting](#fixed-window-rate-limiting)
   - Description
   - Example Usage
   - Parameters
2. [Token Bucket Rate Limiting](#token-bucket-rate-limiting)
   - Description
   - Example Usage
   - Parameters
3. [Common Notes](#common-notes)

---

## <a name="fixed-window-rate-limiting"></a> Fixed Window Rate Limiting

#### Method Signature
```csharp
public static void AddFixedWindowPolicy(this RateLimiterPolicyRegistry registry, string policyName, Action<FixedWindowOptions> configure, bool isGlobal = false)
```

### Description
The `AddFixedWindowPolicy` method registers a **Fixed Window** rate-limiting policy within the provided `RateLimiterPolicyRegistry`. In this strategy, the rate-limiting window resets at fixed intervals, limiting the number of requests allowed during each window.

Developers can configure the policy by specifying:
- **Window Size**: How long the window should last.
- **Request Limit**: How many requests are allowed within that time frame.

This policy can be applied globally across the application if the `isGlobal` flag is set to `true`.

### Example Usage
```csharp
var registry = new RateLimiterPolicyRegistry();
registry.AddFixedWindowPolicy("FixedWindowPolicy", options =>
{
    options.WindowSize = TimeSpan.FromMinutes(1); // Set the duration of the window
    options.RequestLimit = 100;                   // Set the request limit per window
}, isGlobal: true); // Apply globally
```

### Parameters
- **`registry`**: The `RateLimiterPolicyRegistry` object where the policy will be registered.
- **`policyName`**: A string representing the unique name for the policy.
- **`configure`**: A delegate used to configure the `FixedWindowOptions`, such as window size and request limits.
- **`isGlobal`**: A boolean indicating whether the policy should be applied globally in middleware (default is `false`).

---

## <a name="token-bucket-rate-limiting"></a> Token Bucket Rate Limiting

#### Method Signature
```csharp
public static void AddTokenBucketPolicy(this RateLimiterPolicyRegistry registry, string policyName, Action<TokenBucketOptions> configure, bool isGlobal = false)
```

### Description
The `AddTokenBucketPolicy` method registers a **Token Bucket** rate-limiting policy within the provided `RateLimiterPolicyRegistry`. In this strategy, tokens are added to a bucket at a fixed rate, and each request consumes a token. Once the bucket is empty, requests are denied until more tokens are added.

This policy allows for burst traffic, as tokens can accumulate when not used, making it suitable for applications that experience sporadic traffic surges.

### Example Usage
```csharp
var registry = new RateLimiterPolicyRegistry();
registry.AddTokenBucketPolicy("TokenBucketPolicy", options =>
{
    options.TokenLimit = 200;                     // Set the maximum number of tokens
    options.RefillInterval = TimeSpan.FromSeconds(1); // Set the interval for refilling tokens
    options.RefillAmount = 5;                      // Set the number of tokens added at each refill
}, isGlobal: false); // Apply locally
```

### Parameters
- **`registry`**: The `RateLimiterPolicyRegistry` object where the policy will be registered.
- **`policyName`**: A string representing the unique name for the policy.
- **`configure`**: A delegate used to configure the `TokenBucketOptions`, such as token limits and refill rate.
- **`isGlobal`**: A boolean indicating whether the policy should be applied globally in middleware (default is `false`).

---

## <a name="common-notes"></a> Common Notes
- Both methods require a `RateLimiterPolicyRegistry` where policies can be stored and accessed.
- Policies can be applied globally by setting the `isGlobal` flag to `true`, which will apply the rate limiter across all API endpoints.
- The `configure` delegate allows developers to finely control the rate-limiting behavior based on the application’s needs.

---

This documentation provides clear and concise information for developers on how to implement and configure rate-limiting policies using the extension methods provided in the `RateLimiterExtensions` class. If you need any further modifications or additional details, feel free to ask!