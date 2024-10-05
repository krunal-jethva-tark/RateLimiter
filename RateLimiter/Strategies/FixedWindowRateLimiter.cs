using RateLimiter.Interfaces;

namespace RateLimiter.Strategies;

public class FixedWindowRateLimiter: IRateLimiterStrategy
{
    public bool IsRequestAllowed(string key, out RateLimitHeaders headers)
    {
        throw new NotImplementedException();
    }
}