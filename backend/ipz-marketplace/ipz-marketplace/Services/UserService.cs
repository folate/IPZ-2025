using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace ipz_marketplace.Services
{
    public class UserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }

        public IActionResult GetUserCurrentTime()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var timezone = httpContext?.Request.Headers["X-Timezone"].FirstOrDefault();

            if (string.IsNullOrEmpty(timezone))
            {
                Console.WriteLine("Fallback to UTC");
                return new OkObjectResult(DateTime.UtcNow);
            }

            try
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                var userTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
                return new OkObjectResult(userTime);
            }
            catch
            {
                return new BadRequestObjectResult("Invalid timezone");
            }
        }
    }
}
