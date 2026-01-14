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

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSellerData([FromRoute] int id)
    {
        var seller = await _context.Sellers
            .Include(s => s.User)
            .Where(s => s.Id == id)
            .Select(s => new SellerDTO
            {
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

    [Authorize]
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

    [Authorize]
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

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMySellerProfile([FromBody] UpdateSellerDTO updateDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var seller = await _context.Sellers
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (seller == null)
        {
            return NotFound("Seller profile not found");
        }

        seller.Bio = updateDto.Bio ?? seller.Bio;
        seller.Skills = updateDto.Skills ?? seller.Skills;
        seller.HourlyRate = updateDto.HourlyRate ?? seller.HourlyRate;
        seller.PortfolioUrl = updateDto.PortfolioUrl ?? seller.PortfolioUrl;
        seller.IsAvailable = updateDto.IsAvailable ?? seller.IsAvailable;

        await _context.SaveChangesAsync();

        return Ok("Seller profile updated successfully");
    }
}
