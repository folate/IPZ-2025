using ipz_marketplace.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;
        public SearchController(MarketplaceDbContext context) 
        { 
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? search, [FromQuery] string? category, [FromQuery] string? sortBy, [FromQuery] string? order)
        {
            var query = _context.SellerAds
                .Include(p => p.Gigs)
                .Include(p => p.Photos)
                .Include(p => p.Freelancer)
                .Where(p => !p.IsPrivate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => EF.Functions.ILike(p.Title, $"%{search}%")
                                || EF.Functions.ILike(p.Description, $"%{search}%"));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool isDesc = order?.ToLower() == "desc";
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = isDesc ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title);
                        break;
                    case "price":
                        if (isDesc)
                        {
                            query = query.OrderByDescending(p => p.Gigs.Min(g => g.Price));
                        }
                        else
                        {
                            query = query.OrderBy(p => p.Gigs.Min(g => g.Price));
                        }
                        break;
                }
            }

            var content = await query.ToListAsync();

            if (content != null) 
            {
                var result = content.Select(p => new {
                    p.Id,
                    p.Title,
                    p.Description,
                    p.Category,
                    p.CreateDate,
                    p.FreelancerId,
                    Freelancer = p.Freelancer?.UserName,
                    Gigs = p.Gigs.Select(g => new { g.Id, g.TierName, g.TierDescription, g.Price }),
                    Photos = p.Photos.Select(ph => new { ph.Url, ph.IsMain })
                });
                return Ok(result);
            }

            return NotFound();
        }
    }
}
