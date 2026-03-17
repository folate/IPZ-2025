using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("top")]
        public IActionResult GetTop()
        {
            var topCategories = _context.Categories
                .Select(c => new 
                {
                    c.Name,
                    AdsCount = _context.SellerAds.Count(a => a.Category == c.Name) + _context.BuyerAds.Count(a => a.Category == c.Name)
                })
                .OrderByDescending(c => c.AdsCount)
                .Take(15)
                .ToList();
            return Ok(topCategories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] string categoryName)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (existingCategory != null)
            {
                return BadRequest("Category already exists");
            }

            var newCategory = new ipz_marketplace.Entities.Category
            {
                Name = categoryName
            };
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return Ok("Category created successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory([FromBody] string categoryName)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Name == categoryName);
            if (category != null)
            {
                if(category.ToString() == categoryName)
                {
                    _context.Categories.Remove(category);
                    return Ok("Category deleted");
                }
            }
            return BadRequest("Category not found");
        }
    }
}
