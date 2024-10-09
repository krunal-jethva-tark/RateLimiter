using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RateLimiter.Filters;

public class RateLimitingFilter(RateLimiterPolicyRegistry rateLimiterPolicyRegistry) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var policyName = context.ActionDescriptor.EndpointMetadata.OfType<EnableRateLimitingAttribute>()
            .FirstOrDefault()?.PolicyName;

        if (!string.IsNullOrEmpty(policyName) && rateLimiterPolicyRegistry.Policies.ContainsKey(policyName))
        {
            var strategy = rateLimiterPolicyRegistry.Policies[policyName](context.HttpContext);
            var allowed = strategy.IsRequestPermittedAsync(context.HttpContext).Result;

            if (!allowed)
            {
                context.Result = new ContentResult
                {
                    StatusCode = strategy.Options.RejectionStatusCode,
                    Content = strategy.Options.RejectionMessage,
                };
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) {}
}