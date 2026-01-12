using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ipz_marketplace.Services
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly SignInManager<User> _signInManager;
        public AuthService(IServiceProvider serviceProvider, SignInManager<User> signInManager)
        {
            _serviceProvider = serviceProvider;
            _userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Modify(UserModifyDTO userInfo)
        {
            var user = await _userManager.FindByNameAsync(userInfo.Login);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }

            user.FirstName = userInfo.FirstName ?? user.FirstName;
            user.LastName = userInfo.LastName ?? user.LastName;
            user.Email = userInfo.Email ?? user.Email;
            user.IsFreelancer = userInfo.isFreelancer;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userInfo.Password);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new OkObjectResult("User modified successfully");
            }
            return new BadRequestObjectResult(result.Errors);
        }

        public async Task<IActionResult> Register(UserRegisterDTO userInfo)
        {
            var newUser = new User
            {
                UserName = userInfo.Login,
                Email = userInfo.Email,
                EmailConfirmed = true,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                CreateDate = DateTime.UtcNow,
                IsFreelancer = userInfo.isFreelancer
            };

            var result = await _userManager.CreateAsync(newUser, userInfo.Password);

            if (result.Succeeded)
            {
                if(userInfo.isFreelancer)
                    await _userManager.AddToRoleAsync(newUser, "Freelancer");
                else
                    await _userManager.AddToRoleAsync(newUser, "User");

                await _signInManager.SignInAsync(newUser, isPersistent: true);
                return new OkObjectResult("User sucessfuly registered!");
            }
            return new BadRequestObjectResult(result.Errors);
        }

        public async Task<LoginResponse?> Login(string login, string password, bool doNotLogout)
        {
            var userLogin = await _userManager.FindByNameAsync(login);
            if (userLogin == null)
            {
                return null;
            }
            
            var result = await _signInManager.PasswordSignInAsync(userLogin, password, isPersistent: doNotLogout, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return null;
            }

            return new LoginResponse
                {
                    Username = userLogin.UserName,
                    Email = userLogin.Email,
                    Roles = (List<string>)await _userManager.GetRolesAsync(userLogin)
                };
        }
    }
}
