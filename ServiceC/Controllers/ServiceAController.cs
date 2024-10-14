using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers;

[ApiController]
[Route("serviceC")]
public class ServiceAController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello() => Ok("Hello from Service A!");

    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Ping from Service A!");
}