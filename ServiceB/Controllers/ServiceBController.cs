using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ServiceB.Controllers;

[ApiController]
[Route("serviceB")]
public class ServiceBController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello() => Ok("Hello from Service A!");

    [HttpGet("ping")]
    [EnableRateLimiting("ServiceAAttribute")]
    public IActionResult Ping() => Ok("Ping from Service A!");
}