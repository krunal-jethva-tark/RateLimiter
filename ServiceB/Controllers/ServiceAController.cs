using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers;

[ApiController]
[Route("serviceB")]
public class ServiceAController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello() => Ok("Hello from Service A!");

    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Ping from Service A!");
}