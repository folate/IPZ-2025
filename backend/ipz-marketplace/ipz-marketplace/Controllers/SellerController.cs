using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly MarketplaceDbContext _context;

    public SellerController(UserManager<User> userManager, MarketplaceDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSellerData([FromRoute] int id)
    {
        var seller = await _context.Sellers
            .Include(s => s.User)
            .Where(s => s.Id == id)
            .Select(s => new SellerDTO
            {
                Id = s.Id,
                UserId = s.UserId,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                Email = s.User.Email,
                Bio = s.Bio,
                Skills = s.Skills,
                HourlyRate = s.HourlyRate,
                CompletedJobs = s.CompletedJobs,
                Rating = s.Rating,
                TotalReviews = s.TotalReviews,
                JoinedDate = s.JoinedDate,
                PortfolioUrl = s.PortfolioUrl,
                IsAvailable = s.IsAvailable
            })
            .FirstOrDefaultAsync();

        if (seller == null)
        {
            return NotFound("Seller not found");
        }

        return Ok(seller);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMySellerProfile()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var seller = await _context.Sellers
            .Include(s => s.User)
            .Where(s => s.UserId == userId)
            .Select(s => new SellerDTO
            {
                UserId = s.UserId,
                FirstName = s.User.FirstName,
                LastName = s.User.LastName,
                Email = s.User.Email,
                Bio = s.Bio,
                Skills = s.Skills,
                HourlyRate = s.HourlyRate,
                CompletedJobs = s.CompletedJobs,
                Rating = s.Rating,
                TotalReviews = s.TotalReviews,
                JoinedDate = s.JoinedDate,
                PortfolioUrl = s.PortfolioUrl,
                IsAvailable = s.IsAvailable
            })
            .FirstOrDefaultAsync();

        if (seller == null)
        {
            return NotFound("Seller profile not found");
        }

        return Ok(seller);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMySellerProfile([FromBody] UpdateSellerDTO updateDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var seller = await _context.Sellers
            .Where(s => s.UserId == userId)
            .FirstOrDefaultAsync();

        if (seller == null)
        {
            return NotFound("Seller profile not found");
        }

        if (updateDto.FirstName != null) user.FirstName = updateDto.FirstName;
        if (updateDto.LastName != null) user.LastName = updateDto.LastName;
        if (updateDto.Bio != null) seller.Bio = updateDto.Bio;
        if (updateDto.Skills != null) seller.Skills = updateDto.Skills;
        if (updateDto.HourlyRate.HasValue) seller.HourlyRate = updateDto.HourlyRate.Value;
        if (updateDto.PortfolioUrl != null) seller.PortfolioUrl = updateDto.PortfolioUrl;
        if (updateDto.IsAvailable.HasValue) seller.IsAvailable = updateDto.IsAvailable.Value;

        var userUpdateResult = await _userManager.UpdateAsync(user);
        if (!userUpdateResult.Succeeded)
        {
            return BadRequest(userUpdateResult.Errors);
        }

        _context.Sellers.Update(seller);
        await _context.SaveChangesAsync();

        return Ok("Profile updated successfully");
    }

    [Authorize(Roles = "Buyer")]
    [HttpPost]
    public async Task<IActionResult> CreateSellerProfile([FromBody] CreateSellerDTO createDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        // Check if user already has a seller profile
        var existingSeller = await _context.Sellers
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (existingSeller != null)
        {
            return BadRequest("User already has a seller profile");
        }

        var seller = new Seller
        {
            UserId = userId,
            Bio = createDto.Bio,
            Skills = createDto.Skills,
            HourlyRate = createDto.HourlyRate,
            PortfolioUrl = createDto.PortfolioUrl,
            JoinedDate = DateTime.UtcNow,
            CompletedJobs = 0,
            Rating = 0,
            TotalReviews = 0,
            IsAvailable = true
        };

        _context.Sellers.Add(seller);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMySellerProfile), new { }, "Seller profile created successfully");
    }

    [AllowAnonymous]
    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetSellerReviews([FromRoute] int id)
    {
        var reviews = await _context.Reviews
            .Where(r => r.SellerId == id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDTO
            {
                Id = r.Id,
                BuyerName = r.Buyer.User.FirstName + " " + r.Buyer.User.LastName,
                Rating = r.Rating,
                Description = r.Description,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    [Authorize]
    [HttpPost("{id}/reviews")]
    public async Task<IActionResult> AddReview([FromRoute] int id, [FromBody] CreateReviewDTO createDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.UserId == userId);
        if (buyer == null) return BadRequest("Only buyers can leave reviews");

        var seller = await _context.Sellers.FindAsync(id);
        if (seller == null) return NotFound("Seller not found");

        if (seller.UserId == userId)
        {
            return BadRequest("You cannot review your own profile");
        }

        var review = new Review
        {
            SellerId = id,
            BuyerId = buyer.Id,
            Rating = createDto.Rating,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        
        // Update seller rating
        var allReviews = await _context.Reviews.Where(r => r.SellerId == id).ToListAsync();
        decimal totalRating = allReviews.Sum(r => (decimal)r.Rating) + createDto.Rating;
        int reviewCount = allReviews.Count + 1;
        
        seller.Rating = totalRating / reviewCount;
        seller.TotalReviews = reviewCount;

        await _context.SaveChangesAsync();

        return Ok("Review added successfully");
    }
}
