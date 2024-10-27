## EnableRateLimitingAttribute

The `EnableRateLimitingAttribute` is a custom attribute that allows you to apply rate limiting to specific controller actions in an ASP.NET Core application. This attribute enforces rate limiting policies defined elsewhere in the application, ensuring that the specified action is protected from excessive or abusive requests.

### Parameters

- **Policy Name**: The attribute takes a string parameter, which represents the name of the rate limiting policy to be applied.
  - Example: `"MyRateLimitingPolicy"`

### Usage Example

Here’s an example of how to use the `EnableRateLimitingAttribute` in a controller:

```csharp
[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    // Applies the rate limiting policy "MyRateLimitingPolicy" to this action
    [HttpGet]
    [EnableRateLimiting("MyRateLimitingPolicy")]
    public IActionResult Get()
    {
        return Ok("Hello, world!");
    }
}
```