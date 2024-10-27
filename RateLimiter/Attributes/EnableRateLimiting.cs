using Microsoft.AspNetCore.Mvc.Filters;

namespace RateLimiter.Attributes;

/// <summary>
/// An attribute that enables rate limiting on resource actions.
/// This attribute can be used to specify a rate limiting policy name for the action it decorates.
/// </summary>
public sealed class EnableRateLimiting(string? policyName) : Attribute
{
    /// <summary>
    /// Gets the name of the rate limiting policy to apply.
    /// </summary>
    public string? PolicyName { get; private set; } = policyName;
    
    // /// <summary>
    // /// Executes before the resource method is called.
    // /// This method is intentionally left empty because rate limiting is managed by middleware.
    // /// </summary>
    // /// <param name="context">The context of the resource being executed.</param>
    // public void OnResourceExecuting(ResourceExecutingContext context)
    // {
    //     // This method is intentionally left empty
    //     // We will handle rate limiting in the middleware
    // }
    //
    // /// <summary>
    // /// Executes after the resource method has been called.
    // /// This method is intentionally left empty.
    // /// </summary>
    // /// <param name="context">The context of the resource executed.</param>
    // public void OnResourceExecuted(ResourceExecutedContext context)
    // {
    //     // This method is intentionally left empty
    // }
}