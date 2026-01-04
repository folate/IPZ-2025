using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;
        public AuthController(SignInManager<User> signInManager, IServiceProvider serviceProvider)
        {
            _signInManager = signInManager;
            _serviceProvider = serviceProvider;
            _userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userInfo)
        {
            var userLogin = await _userManager.FindByNameAsync(userInfo.Login);

            if (userLogin == null)
            {
                return Unauthorized();
            }
            var result = await _userManager.CheckPasswordAsync(userLogin, userInfo.Password);

            if (!result)
            {
                return Unauthorized();
            }
            return Ok("Login success: " + userInfo.Login);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userInfo)
        {
            var newUser = new User
            {
                UserName = userInfo.Login,
                Email = userInfo.Email,
                EmailConfirmed = true,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                CreateDate = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsFreelancer = userInfo.isFreelancer
            };

            var result = await _userManager.CreateAsync(newUser, userInfo.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "User");
                return Ok("Registration Succeeded");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("modify")]
        public async Task<IActionResult> Modify([FromBody] UserModifyDTO userInfo)
        {
            var user = await _userManager.FindByNameAsync(userInfo.Login);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.FirstName = userInfo.FirstName ?? user.FirstName;
            user.LastName = userInfo.LastName ?? user.LastName;
            user.Email = userInfo.Email ?? user.Email;
            user.IsFreelancer = userInfo.isFreelancer;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userInfo.Password);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("User modified successfully");
            }
            return BadRequest(result.Errors);
        }
    }
}
