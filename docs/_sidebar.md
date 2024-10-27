- [Home](/RateLimiter/)
- [Introduction](/RateLimiter/README.md#introduction)
- [Features](/RateLimiter/README.md#features)
- Documentation
  - [RateLimitingMiddleware](/RateLimiter/RateLimitingMiddleware.md)
    - [Features](/RateLimiter/RateLimitingMiddleware.md#features)
    - [Example Usage](/RateLimiter/RateLimitingMiddleware.md#example-usage)
    - [Handling Limits and Responses](/RateLimiter/RateLimitingMiddleware.md#handling-limits-and-responses)
    - [When to Use](/RateLimiter/RateLimitingMiddleware.md#when-to-use)
  - Strategies
    - [Fixed Window Strategy](/RateLimiter/FixedWindowRateStrategy.md#fixed-window-rate-limiting-strategy)
      - [Example](/RateLimiter/FixedWindowRateStrategy.md#example-how-to-use-fixedwindowratestrategy)
      - [When to Use](/RateLimiter/FixedWindowRateStrategy.md#when-to-use)
      - [Fixed Window Options](/RateLimiter/FixedWindowOptions.md)
    - [Token Bucket Strategy](/RateLimiter/TokenBucketRateStrategy.md#token-bucket-rate-limiting-strategy)
      - [Example](/RateLimiter/TokenBucketRateStrategy.md#example-how-to-use-tokenbucketratestrategy)
      - [When to Use](/RateLimiter/TokenBucketRateStrategy.md#when-to-use)
      - [Token Bucket Options](/RateLimiter/TokenBucketOptions.md)

  - Advanced Configuration Options
    - [Per-User, Per-IP, and Per-Service Rate Limiting](/RateLimiter/AdvancedConfigurationOptions.md#per-user-per-ip-and-per-service-rate-limiting)
    - [Combining Strategies](/RateLimiter/CombiningStrategy.md#combining-rate-limiting-strategies)

  - Attributes
    - [DisableRateLimitingAttribute](/RateLimiter/DisableRateLimitingAttribute.md#disableratelimitingattribute)
    - [EnableRateLimitingAttribute](/RateLimiter/EnableRateLimitingAttribute.md#enableratelimitingattribute)

  - Counter Store
    - [In-Memory Rate Limit Store](/RateLimiter/InMemoryRateLimitCounterStore.md)
      - [Key Features](/RateLimiter/InMemoryRateLimitCounterStore.md#key-features)
      - [When to Use](/RateLimiter/InMemoryRateLimitCounterStore.md#when-to-use)
      - [Considerations for Production](/RateLimiter/InMemoryRateLimitCounterStore.md#considerations-for-production)
    - [Redis Rate Limit Store](/RateLimiter/RedisRateLimitCounterStore.md#redis-rate-limit-store-redisratelimitcounterstore)
      - [Features]/RateLimiter/(RedisRateLimitCounterStore.md#features)
      - [When to Use](/RateLimiter/RedisRateLimitCounterStore.md#when-to-use)
      - [Prerequisites](/RateLimiter/RedisRateLimitCounterStore.md#prerequisites)
      - [Example](/RateLimiter/RedisRateLimitCounterStore.md#example)

  - Interfaces
    - [IRateLimitCounterStore](/RateLimiter/IRateLimitCounterStore.md#iratelimitcounterstore-interface)
      - [Example Usage](/RateLimiter/IRateLimitCounterStore.md#example-usage)
      - [Purpose](/RateLimiter/IRateLimitCounterStore.md#purpose)

  - Data Models
    - [RateLimitData](/RateLimiter/RateLimitData.md#rate-limit-data-ratelimitdata)

  - Dependency Injection
    - [StartupExtensions](/RateLimiter/StartupExtensions.md#registering-rate-limiter-in-dependency-injection)
      - [Example](/RateLimiter/StartupExtensions.md#example)
      - [Explanation](/RateLimiter/StartupExtensions.md#explanation)
    - [Rate Limiter Extension Methods](/RateLimiter/RateLimiterExtensions.md)
      - [AddFixedWindowPolicy](/RateLimiter/RateLimiterExtensions.md#addfixedwindowpolicy)
      - [AddTokenBucketPolicy](/RateLimiter/RateLimiterExtensions.md#addtokenbucketpolicy)
      - [MarkAsDefault](/RateLimiter/RateLimiterExtensions.md#markasdefault)

- [Example](/RateLimiter/README.md#example)    