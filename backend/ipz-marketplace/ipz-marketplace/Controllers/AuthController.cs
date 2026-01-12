using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using ipz_marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userInfo)
        {
            var loginResponse = await _authService.Login(userInfo.Login, userInfo.Password, userInfo.doNotLogout);
            if (loginResponse == null)
            {
                return Unauthorized("Invalid login or password.");
            }
            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userInfo)
        {
            return await _authService.Register(userInfo);
        }

        [HttpPost("modify")]
        public async Task<IActionResult> Modify([FromBody] UserModifyDTO userInfo)
        {
            return await _authService.Modify(userInfo);
        }
    }
}
