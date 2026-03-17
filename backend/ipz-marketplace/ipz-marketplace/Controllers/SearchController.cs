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
        public async Task<IActionResult> Index([FromQuery] string? search, [FromQuery] string? category)
        {
            var query = _context.SellerAds.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => EF.Functions.ILike(p.Title, $"%{search}%")
                                || EF.Functions.ILike(p.Description, $"%{search}%"));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            var content = await query.ToListAsync();

            if (content != null) 
                return Ok(content);

            return NotFound();
        }
    }
}
