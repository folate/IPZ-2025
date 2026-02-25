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
        public ActionResult Index([FromQuery] string search)
        {
            var content = _context.SellerAds
                .AsNoTracking()
                .Select(
                    a => new ListingSellerAdDTO
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        Freelancer = a.Freelancer.UserName,
                        Gigs = a.Gigs.Select(g => new GigsDTO
                        {
                            Id = g.Id,
                            TierName = g.TierName,
                            TierDescription = g.TierDescription,
                            Price = g.Price
                        }).ToList()
                    })
                .Where(p => EF.Functions.ILike(p.Title, $"%{search}%") || EF.Functions.ILike(p.Description, $"%{search}%"))
                .Take(10)
                .ToList();
            if(content != null)
                return Ok(content);
            return BadRequest();
        }
    }
}
