- [Home](/)
- [Introduction](README.md#introduction)
- [Features](README.md#features)
- Documentation
  - [RateLimitingMiddleware](RateLimitingMiddleware.md)
    - [Features](RateLimitingMiddleware.md#features)
    - [Example Usage](RateLimitingMiddleware.md#example-usage)
    - [Handling Limits and Responses](RateLimitingMiddleware.md#handling-limits-and-responses)
    - [When to Use](RateLimitingMiddleware.md#when-to-use)
  - Strategies
    - [Fixed Window Strategy](FixedWindowRateStrategy.md#fixed-window-rate-limiting-strategy)
      - [Example](FixedWindowRateStrategy.md#example-how-to-use-fixedwindowratestrategy)
      - [When to Use](FixedWindowRateStrategy.md#when-to-use)
      - [Fixed Window Options](FixedWindowOptions.md)
    - [Token Bucket Strategy](TokenBucketRateStrategy.md#token-bucket-rate-limiting-strategy)
      - [Example](TokenBucketRateStrategy.md#example-how-to-use-tokenbucketratestrategy)
      - [When to Use](TokenBucketRateStrategy.md#when-to-use)
      - [Token Bucket Options](TokenBucketOptions.md)

  - Advanced Configuration Options
    - [Per-User, Per-IP, and Per-Service Rate Limiting](AdvancedConfigurationOptions.md#per-user-per-ip-and-per-service-rate-limiting)
    - [Combining Strategies](CombiningStrategy.md#combining-rate-limiting-strategies)

  - Attributes
    - [DisableRateLimitingAttribute](DisableRateLimitingAttribute.md#disableratelimitingattribute)
    - [EnableRateLimitingAttribute](EnableRateLimitingAttribute.md#enableratelimitingattribute)

  - Counter Store
    - [In-Memory Rate Limit Store](InMemoryRateLimitCounterStore.md)
      - [Key Features](InMemoryRateLimitCounterStore.md#key-features)
      - [When to Use](InMemoryRateLimitCounterStore.md#when-to-use)
      - [Considerations for Production](InMemoryRateLimitCounterStore.md#considerations-for-production)
    - [Redis Rate Limit Store](RedisRateLimitCounterStore.md#redis-rate-limit-store-redisratelimitcounterstore)
      - [Features](RedisRateLimitCounterStore.md#features)
      - [When to Use](RedisRateLimitCounterStore.md#when-to-use)
      - [Prerequisites](RedisRateLimitCounterStore.md#prerequisites)
      - [Example](RedisRateLimitCounterStore.md#example)

  - Interfaces
    - [IRateLimitCounterStore](IRateLimitCounterStore.md#iratelimitcounterstore-interface)
      - [Example Usage](IRateLimitCounterStore.md#example-usage)
      - [Purpose](IRateLimitCounterStore.md#purpose)

  - Data Models
    - [RateLimitData](RateLimitData.md#rate-limit-data-ratelimitdata)

  - Dependency Injection
    - [StartupExtensions](StartupExtensions.md#registering-rate-limiter-in-dependency-injection)
      - [Example](StartupExtensions.md#example)
      - [Explanation](StartupExtensions.md#explanation)
    - [Rate Limiter Extension Methods](RateLimiterExtensions.md)
      - [AddFixedWindowPolicy](RateLimiterExtensions.md#addfixedwindowpolicy)
      - [AddTokenBucketPolicy](RateLimiterExtensions.md#addtokenbucketpolicy)
      - [MarkAsDefault](RateLimiterExtensions.md#markasdefault)

- [Example](README.md#example)    