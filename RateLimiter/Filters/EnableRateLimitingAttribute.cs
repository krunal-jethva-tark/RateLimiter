using Microsoft.AspNetCore.Mvc.Filters;

namespace RateLimiter.Filters;

public class EnableRateLimitingAttribute(string? policyName) : Attribute, IResourceFilter
{
    public string? PolicyName { get; private set; } = policyName;
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        // This method is intentionally left empty
        // We will handle rate limiting in the middleware
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        // This method is intentionally left empty
    }
}