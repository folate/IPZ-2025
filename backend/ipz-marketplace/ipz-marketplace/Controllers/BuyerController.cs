using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ipz_marketplace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuyerController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly MarketplaceDbContext _context;

    public BuyerController(UserManager<User> userManager, MarketplaceDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBuyerData([FromRoute] int id)
    {
        var buyer = await _context.Buyers
            .Include(b => b.User)
            .Where(b => b.Id == id)
            .Select(b => new BuyerDTO
            {
                FirstName = b.User.FirstName,
                LastName = b.User.LastName,
                Email = b.User.Email,
                ShippingAddress = b.ShippingAddress,
                BillingAddress = b.BillingAddress,
                TotalOrders = b.TotalOrders,
                JoinedDate = b.JoinedDate,
                LastOrderDate = b.LastOrderDate,
                PreferredPaymentMethod = b.PreferredPaymentMethod
            })
            .FirstOrDefaultAsync();

        if (buyer == null)
        {
            return NotFound("Buyer not found");
        }

        return Ok(buyer);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyBuyerProfile()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var buyer = await _context.Buyers
            .Include(b => b.User)
            .Where(b => b.UserId == userId)
            .Select(b => new BuyerDTO
            {
                FirstName = b.User.FirstName,
                LastName = b.User.LastName,
                Email = b.User.Email,
                ShippingAddress = b.ShippingAddress,
                BillingAddress = b.BillingAddress,
                TotalOrders = b.TotalOrders,
                JoinedDate = b.JoinedDate,
                LastOrderDate = b.LastOrderDate,
                PreferredPaymentMethod = b.PreferredPaymentMethod
            })
            .FirstOrDefaultAsync();

        if (buyer == null)
        {
            return NotFound("Buyer profile not found");
        }

        return Ok(buyer);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyBuyerProfile([FromBody] UpdateBuyerDTO updateDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return Unauthorized();
        }

        var buyer = await _context.Buyers
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (buyer == null)
        {
            return NotFound("Buyer profile not found");
        }

        buyer.ShippingAddress = updateDto.ShippingAddress ?? buyer.ShippingAddress;
        buyer.BillingAddress = updateDto.BillingAddress ?? buyer.BillingAddress;
        buyer.PreferredPaymentMethod = updateDto.PreferredPaymentMethod ?? buyer.PreferredPaymentMethod;

        await _context.SaveChangesAsync();

        return Ok("Buyer profile updated successfully");
    }
}