using Microsoft.AspNetCore.Mvc;
using RateLimiter.Filters;

namespace ServiceA.Controllers;

[ApiController]
[Route("serviceA")]
public class ServiceAController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello() => Ok("Hello from Service A!");

    [HttpGet("ping")]
    [EnableRateLimiting("ServiceAAttribute")]
    public IActionResult Ping() => Ok("Ping from Service A!");

    [HttpGet("echo")]
    public IActionResult Echo() => Ok();
}