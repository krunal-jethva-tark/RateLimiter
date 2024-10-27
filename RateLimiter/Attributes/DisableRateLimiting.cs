using Microsoft.AspNetCore.Mvc.Filters;

namespace RateLimiter.Attributes;

/// <summary>
/// An attribute that disables rate limiting on resource actions.
/// This attribute can be used to exclude specific actions from rate limiting policies.
/// </summary>
public sealed class DisableRateLimiting: Attribute
{
}