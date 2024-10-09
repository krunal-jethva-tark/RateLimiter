namespace RateLimiter.Filters;

public class EnableRateLimitingAttribute(string policyName) : Attribute
{
    public string PolicyName { get; private set; } = policyName;
}