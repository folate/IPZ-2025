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
    public class AdController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MarketplaceDbContext _context;
        public AdController(UserManager<User> userManager, MarketplaceDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Freelancer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAd([FromBody] SellerAdDTO adDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("testing backend and userId here: " + userId);
            }

            var ad = new SellerAd
            {
                Title = adDto.Title,
                Description = adDto.Description,
                CreateDate = DateTime.UtcNow,
                FreelancerId = userId,
                Gigs = adDto.Gigs.Select(g => new Gigs
                {
                    TierName = g.TierName,
                    TierDescription = g.TierDescription,
                    Price = g.Price
                }).ToList()
            };

            _context.SellerAds.Add(ad);
            await _context.SaveChangesAsync();

            return Ok(userId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAd([FromRoute] int id)
        {
            var ad = _context.SellerAds
                .Select(a => new ListingSellerAdDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList()
                })
                .FirstOrDefault(a => a.Id == id);
            return Ok(ad);
        }
    }
}
