using Microsoft.AspNetCore.Http;

namespace RateLimiter.Models;

/// <summary>
/// Base options class for configuring rate limiting strategies.
/// This class provides configuration options common to all rate limiting strategies.
/// </summary>
public class RateLimiterStrategyOptions
{
    /// <summary>
    /// Gets or sets the key generator function used to create keys for rate limiting.
    /// The default key generator is set to generate keys based on the client's IP address.
    /// </summary>
    /// <value>
    /// A function that takes an <see cref="HttpContext"/> and returns a string representing the key for rate limiting.
    /// This key is used to uniquely identify the requests made by a client.
    /// </value>
    public Func<HttpContext, string> KeyGenerator { get; set; } = KeyGenerators.KeyGenerator.IP;

    /// <summary>
    /// Gets or sets the HTTP status code returned for rejected requests due to rate limiting.
    /// The default value is set to <c>429 Too Many Requests</c>.
    /// </summary>
    /// <value>
    /// An integer representing the HTTP status code for rejected requests.
    /// This status code is sent back to the client when the rate limit is exceeded.
    /// </value>
    public int RejectionStatusCode { get; set; } = StatusCodes.Status429TooManyRequests;

    /// <summary>
    /// Gets or sets the response message returned for rejected requests due to rate limiting.
    /// The default message is "Rate limit exceeded. Please try again later."
    /// </summary>
    /// <value>
    /// A string representing the message returned to the client when a request is rejected.
    /// This message provides feedback to the client regarding the rejection.
    /// </value>
    public string RejectionMessage { get; set; } = "Rate limit exceeded. Please try again later.";
}