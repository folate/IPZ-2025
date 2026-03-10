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
    public class SellerAdController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MarketplaceDbContext _context;
        public SellerAdController(UserManager<User> userManager, MarketplaceDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAd([FromBody] SellerAdDTO adDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            var ad = new SellerAd
            {
                Title = adDto.Title,
                Description = adDto.Description,
                CreateDate = DateTime.UtcNow,
                FreelancerId = userId,
                Category = adDto.Category,
                Gigs = adDto.Gigs.Select(g => new Gigs
                {
                    Id = g.Id,
                    TierName = g.TierName,
                    TierDescription = g.TierDescription,
                    Price = g.Price
                }).ToList()
            };

            _context.SellerAds.Add(ad);
            await _context.SaveChangesAsync();

            return Ok("Sucessfuly created!");
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
                    Freelancer = a.Freelancer.UserName,
                    Category = a.Category,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        Id = g.Id,
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList()
                })
                .FirstOrDefault(a => a.Id == id);
            return Ok(ad);
        }
        [HttpGet("all/{number}")]
        public async Task<IActionResult> GetFewAds([FromRoute] int number)
        {
            List<ListingSellerAdDTO> sellerAds = _context.SellerAds
                .Select(a => new ListingSellerAdDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Freelancer = a.Freelancer.UserName,
                    FreelancerId = a.FreelancerId,
                    Category = a.Category,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        Id = g.Id,
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList()
                })
                .Take(number)
                .ToList();

            return Ok(sellerAds);
        }

        [HttpGet("UserAds")]
        public async Task<IActionResult> GetUsersAds()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            List<ListingSellerAdDTO> sellerAds = _context.SellerAds
                .Where(a => a.FreelancerId == userId)
                .Select(a => new ListingSellerAdDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Freelancer = a.Freelancer.UserName,
                    Category = a.Category,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList()
                })
                .ToList();
            return Ok(sellerAds);
        }
    }
}
