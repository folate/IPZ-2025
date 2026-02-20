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
    public class BuyerAdController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MarketplaceDbContext _context;
        public BuyerAdController(UserManager<User> userManager, MarketplaceDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAd([FromBody] BuyerAdDTO adDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            var ad = new BuyerAd
            {
                Title = adDto.Title,
                Description = adDto.Description,
                Buyer = await _userManager.FindByIdAsync(userId),
                CreateDate = DateTime.UtcNow,
                Deadline = adDto.Deadline,
                Category = adDto.Category,
                Budget = adDto.Budget
            };

            _context.BuyerAds.Add(ad);
            await _context.SaveChangesAsync();

            return Ok("Sucessfuly created!");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuyerAd([FromRoute] int id)
        {
            var ad = _context.BuyerAds.Select(a => new ListingBuyerAdDTO
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                BuyerName = a.Buyer.UserName ?? string.Empty,
                CreateDate = a.CreateDate,
                Deadline = a.Deadline,
                Category = a.Category,
                Budget = a.Budget,
            }).FirstOrDefault(a => a.Id == id);

            if (ad == null) return NoContent();

            return Ok(ad);
        }
        [HttpGet("all/{number}")]
        public async Task<IActionResult> GetFewAds([FromRoute]int number)
        {
            var userId = _userManager.GetUserId(User);
            if(userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            List<BuyerAd> ads = _context.BuyerAds.Where(a => a.Buyer.Id != userId).Take(number).ToList();

            return Ok(ads);
        }
    }
}
