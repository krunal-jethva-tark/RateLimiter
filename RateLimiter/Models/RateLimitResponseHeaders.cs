namespace RateLimiter.Models;

public class RateLimitResponseHeaders
{
    public string Limit { get; set; }
    public string Remaining { get; set; }
    public string Reset { get; set; }
}