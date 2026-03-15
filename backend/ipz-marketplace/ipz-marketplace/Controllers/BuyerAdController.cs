using FluentEmail.Core.Models;
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
        private readonly EmailService _emailService;
        public BuyerAdController(UserManager<User> userManager, MarketplaceDbContext context, EmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
        }

        [Authorize(Roles = "Buyer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAd([FromBody] BuyerAdDTO adDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            var logged = await _userManager.FindByIdAsync(userId);
            if(logged == null)
            {
                return NotFound();
            }

            var ad = new BuyerAd
            {
                Title = adDto.Title,
                Description = adDto.Description,
                Buyer = logged,
                CreateDate = DateTime.UtcNow,
                Deadline = adDto.Deadline,
                Category = adDto.Category,
                Budget = adDto.Budget
            };

            await _emailService.EmailConnection(logged.Email, "Sucessfuly created Ad!", $"Thank you {logged.UserName} for creating Ad for our service.");

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
        public async Task<IActionResult> GetFewAds([FromRoute] int number)
        {
            List<BuyerAd> ads = _context.BuyerAds.Take(number).ToList();

            if (ads.Count == 0)
                return NoContent();
            return Ok(ads);
        }

        [HttpGet("UserAds")]
        public async Task<IActionResult> GetUserAds()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            var ads = _context.BuyerAds.Where(a => a.Buyer.Id == userId).ToList();

            if (ads.Count == 0)
                return NoContent();
            return Ok(ads);
        }
    }
}
