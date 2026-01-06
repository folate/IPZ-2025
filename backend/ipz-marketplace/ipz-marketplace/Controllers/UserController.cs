using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using ipz_marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly UserService _userService;
    private readonly MarketplaceDbContext _context;

    public UserController(MarketplaceDbContext context, UserManager<User> userManager)
    {
        _userManager = userManager;
        _userService = new UserService();
        _context = context;
    }

    [HttpGet("getBirthdate/{login}")]
    public async Task<IActionResult> GetUserById([FromRoute] string login)
    {
        var user = await _userManager.FindByNameAsync(login);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.CreateDate);
    }
    
    [HttpGet("getUserCurrentTime")]
    public IActionResult GetUserCurrentTime()
    {
        return (_userService.GetUserCurrentTime());
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("getUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

}