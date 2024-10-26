using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace RateLimiter;

public sealed class RateLimitingMetrics : IDisposable
{
    public const string MeterName = "RateLimiting";

    private readonly Meter _meter;
    private readonly UpDownCounter<long> _activeRequestLeasesCounter;
    private readonly Histogram<double> _requestLeaseDurationCounter;
    private readonly Counter<long> _requestsCounter;

    public RateLimitingMetrics()
    {
        _meter = new Meter(MeterName);

        _activeRequestLeasesCounter = _meter.CreateUpDownCounter<long>(
            "rate_limiting.active_request_leases",
            unit: "{request}",
            description: "Number of HTTP requests that are currently active on the server that hold a rate limiting lease.");

        _requestLeaseDurationCounter = _meter.CreateHistogram<double>(
            "rate_limiting.request_lease.duration",
            unit: "s",
            description: "The duration of rate limiting leases held by HTTP requests on the server.");

        _requestsCounter = _meter.CreateCounter<long>(
            "rate_limiting.requests",
            unit: "{request}",
            description: "Number of requests that tried to acquire a rate limiting lease. Requests could be rejected by global or endpoint rate limiting policies. Or the request could be canceled while waiting for the lease.");
    }

    public void LeaseFailed(string? policyName, RequestRejectionReason reason)
    {
        if (_requestsCounter.Enabled)
        {
            LeaseFailedCore(policyName, reason);
        }
    }
    
    private void LeaseFailedCore(string? policyName, RequestRejectionReason reason)
    {
        var tags = new TagList();
        InitializeRateLimitingTags(ref tags, policyName);
        tags.Add("rate_limiting.result", GetResult(reason));
        _requestsCounter.Add(1, tags);
    }

    public void LeaseStart(string? policyName)
    {
        LeaseStartCore(policyName);
    }

    private void LeaseStartCore(string? policyName)
    {
        var tags = new TagList();
        InitializeRateLimitingTags(ref tags, policyName);
        _activeRequestLeasesCounter.Add(1, tags);
    }
    
    public void LeaseEnd(string? policyName, TimeSpan duration)
    {
        LeaseEndCore(policyName, duration);
    }
    
    private void LeaseEndCore(string? policyName, TimeSpan duration)
    {
        var tags = new TagList();
        InitializeRateLimitingTags(ref tags, policyName);
        _activeRequestLeasesCounter.Add(-1, tags);
        _requestLeaseDurationCounter.Record(duration.TotalSeconds, tags);
        
        tags.Add("rate_limiting.result", "acquired");
        _requestsCounter.Add(1, tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
    
    private static void InitializeRateLimitingTags(ref TagList tags, string? policyName)
    {
        if (policyName is not null)
        {
            tags.Add("rate_limiting.policy", policyName);
        }
    }
    
    private static string GetResult(RequestRejectionReason reason)
    {
        return reason switch
        {
            RequestRejectionReason.EndpointLimiter => "endpoint_limiter",
            RequestRejectionReason.GlobalLimiter => "global_limiter",
            RequestRejectionReason.RequestCanceled => "request_canceled",
            _ => throw new InvalidOperationException("Unexpected value: " + reason)
        };
    }
}

public enum RequestRejectionReason
{
    EndpointLimiter,
    GlobalLimiter,
    RequestCanceled
}
