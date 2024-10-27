using Microsoft.AspNetCore.Mvc;
using RateLimiter.Attributes;

namespace ServiceA.Controllers;

[ApiController]
[Route("serviceA")]
public class ServiceAController : ControllerBase
{
    [HttpGet("hello")]
    [DisableRateLimiting]
    public IActionResult Hello() => Ok("Hello from Service A!");

    [HttpGet("ping")]
    [EnableRateLimiting("ServiceAAttribute")]
    public IActionResult Ping() => Ok("Ping from Service A!");

    [HttpGet("echo")]
    public IActionResult Echo() => Ok();
}