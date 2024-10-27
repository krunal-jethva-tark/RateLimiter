## Per-User, Per-IP, and Per-Service Rate Limiting

You can configure rate limiting strategies based on specific criteria, such as per user, per IP address, or per service. This allows for more granular control over who or what is consuming your API resources.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRateLimiter(registry =>
    {
        // Per-User Rate Limiting
        registry.AddFixedWindowPolicy("UserPolicy", options =>
        {
            options.PermitLimit = 100;
            options.Window = TimeSpan.FromMinutes(1);
            tokenBucketOptions.KeyGenerator = KeyGenerator.User
        });

        // Per-IP Rate Limiting
        registry.AddFixedWindowPolicy("IPPolicy", options =>
        {
            options.PermitLimit = 50;
            options.Window = TimeSpan.FromMinutes(1);
            tokenBucketOptions.KeyGenerator = KeyGenerator.IP;
        });

        // Per-Service Rate Limiting
        registry.AddTokenBucketPolicy("ServicePolicy", options =>
        {
            options.MaxRequestsPerSecond = 20;
            options.BurstCapacity = 100;
            tokenBucketOptions.KeyGenerator = KeyGenerator.Service
        });

        // Adding custom key generation
        registry.AddTokenBucketPolicy("ServicePolicy", options =>
        {
            options.MaxRequestsPerSecond = 20;
            options.BurstCapacity = 100;
            tokenBucketOptions.KeyGenerator = context => return $"custom-key";
        });
    });
}
```

### How it Works

- **Per-User**: Each authenticated user has their own rate limit, helping to prevent individual users from overwhelming the system.
- **Per-IP**: Requests are limited based on the client’s IP address, useful for mitigating abuse from specific IPs.
- **Per-Service**: When inter-service communication is involved, this allows each service to have its own rate limiting quota, useful in microservice architectures.
- **Custom Key Generation**: Allows for custom logic to generate keys for rate limiting. This is useful when you need to implement a specific rate limiting strategy that doesn't fit into the standard per-user, per-IP, or per-service categories. For example, you can generate keys based on specific request attributes or custom business logic.
