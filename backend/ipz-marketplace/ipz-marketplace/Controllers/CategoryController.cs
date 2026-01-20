using Microsoft.AspNetCore.Mvc;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;
        public CategoryController(MarketplaceDbContext context) 
        { 
            _context = context;
        }
        [HttpGet]
        public IActionResult GetCategories()
        {
            var result = _context.Categories.Select(c => new 
            {
                c.Name
            }).ToList();

            if (result == null)
            {
                return NotFound("No categories found");
            }
            return Ok(result);
        }
    }
}
