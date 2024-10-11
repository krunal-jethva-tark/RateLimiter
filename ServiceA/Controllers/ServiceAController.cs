using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceAController: ControllerBase
{
    private static int _totalRequests = 0;
    private static int _acceptedRequests = 0;
    private static int _rejectedRequests = 0;

    [HttpGet("hello-world")]
    public IActionResult HelloWorld()
    {
        Interlocked.Increment(ref _totalRequests);
        Interlocked.Increment(ref _acceptedRequests);
        return Ok("Hello, World!");
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        Interlocked.Increment(ref _totalRequests);
        Interlocked.Increment(ref _acceptedRequests);
        return Ok("Pong");
    }

    // Additional methods to log statistics
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(new
        {
            TotalRequests = _totalRequests,
            AcceptedRequests = _acceptedRequests,
            RejectedRequests = _rejectedRequests
        });
    }
}