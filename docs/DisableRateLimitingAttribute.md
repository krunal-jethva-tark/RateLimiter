## DisableRateLimitingAttribute

The `DisableRateLimitingAttribute` is a custom attribute that allows you to disable rate limiting for specific controller actions in an ASP.NET Core application. This attribute ensures that the specified action is exempt from any rate limiting policies defined elsewhere in the application.  

### Example
Here’s an example of how to use the DisableRateLimitingAttribute in a controller:  
```csharp 
[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase 
{ 
    // Disables rate limiting for this action 
    [HttpGet] 
    [DisableRateLimiting] 
    public IActionResult Get() 
    { 
        return Ok("Rate limiting is disabled for this action."); 
    } 
} 
```  
 
### Notes
- Applying this attribute to a controller action will override any rate limiting policies that would otherwise apply to that action.
- Use this attribute with caution, as it can expose the action to potential abuse or excessive requests.