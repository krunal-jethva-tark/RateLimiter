namespace RateLimiter.Interfaces;

public interface IRateLimiterStrategy
{
    bool IsRequestAllowed(string key, out RateLimitHeaders headers);
}

public class RateLimitHeaders
{
    public int RemainingRequests { get; set; }
    public int MaxRequests { get; set; }
    public int RetryAfterSeconds { get; set; }
}