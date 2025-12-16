using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ipz_marketplace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserManager<User> userManager) : ControllerBase
{
    [HttpGet("getBirthdate/{login}")]
    public async Task<IActionResult> GetUserById([FromRoute] string login)
    {
        var user = await userManager.FindByNameAsync(login);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.DateOfBirth);
    }
    
    [HttpGet("getUserCurrentTimez")]
    public IActionResult GetUserCurrentTime()
    {
        var timezone = HttpContext.Request.Headers["X-Timezone"].FirstOrDefault();
    
        if (string.IsNullOrEmpty(timezone))
        {
            Console.WriteLine("Fallback to UTC");
            return Ok(DateTime.UtcNow);
        }

        try
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            var userTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
            return Ok(userTime);
        }
        catch
        {
            return BadRequest("Invalid timezone");
        }
    }
}