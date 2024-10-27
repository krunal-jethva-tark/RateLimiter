using Microsoft.AspNetCore.Mvc;

namespace ServiceC.Controllers;

[ApiController]
[Route("serviceC")]
public class ServiceCController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello() => Ok("Hello from Service A!");

    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Ping from Service A!");
}