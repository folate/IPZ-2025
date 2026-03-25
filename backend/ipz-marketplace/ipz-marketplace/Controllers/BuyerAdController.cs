using FluentEmail.Core.Models;
using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using ipz_marketplace.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuyerAdController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MarketplaceDbContext _context;
        private readonly EmailService _emailService;
        public BuyerAdController(UserManager<User> userManager, MarketplaceDbContext context, EmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
        }

        [Authorize(Roles = "Buyer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAd([FromBody] BuyerAdDTO adDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            var logged = await _userManager.FindByIdAsync(userId);
            if(logged == null)
            {
                return NotFound();
            }

            var ad = new BuyerAd
            {
                Title = adDto.Title,
                Description = adDto.Description,
                Buyer = logged,
                CreateDate = DateTime.UtcNow,
                Deadline = adDto.Deadline,
                Category = adDto.Category,
                Budget = adDto.Budget,
                IsClosed = false
            };

            await _emailService.EmailConnection(logged.Email, "Sucessfuly created Ad!", $"Thank you {logged.UserName} for creating Ad for our service.");

            _context.BuyerAds.Add(ad);
            await _context.SaveChangesAsync();

            return Ok("Sucessfuly created!");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuyerAd([FromRoute] int id)
        {
            var userId = _userManager.GetUserId(User);

            var ad = await _context.BuyerAds
                .Include(a => a.Buyer)
                .Include(a => a.Offers)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null) return NoContent();

            // Privacy Logic: If closed, only owner or accepted freelancer can see it
            if (ad.IsClosed)
            {
                if (userId == null) return Unauthorized();
                
                var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);
                bool isAcceptedFreelancer = ad.AcceptedOfferId.HasValue && 
                                            ad.Offers.Any(o => o.Id == ad.AcceptedOfferId && o.FreelancerId == seller?.Id);
                
                if (ad.Buyer.Id != userId && !isAcceptedFreelancer)
                {
                    return Forbid();
                }
            }

            var adDto = new ListingBuyerAdDTO
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                BuyerName = ad.Buyer.UserName ?? string.Empty,
                BuyerUserId = ad.Buyer.Id,
                CreateDate = ad.CreateDate,
                Deadline = ad.Deadline,
                Category = ad.Category,
                Budget = ad.Budget,
                IsClosed = ad.IsClosed,
            };

            return Ok(adDto);
        }

        [HttpGet("all/{number}")]
        public async Task<IActionResult> GetFewAds([FromRoute] int number)
        {
            // Only return open ads for the public feed
            List<BuyerAd> ads = await _context.BuyerAds
                .Where(a => !a.IsClosed)
                .Take(number)
                .ToListAsync();

            if (ads.Count == 0)
                return NoContent();
            return Ok(ads);
        }

        [HttpGet("UserAds")]
        public async Task<IActionResult> GetUserAds()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            var ads = await _context.BuyerAds
                .Where(a => a.Buyer.Id == userId)
                .Select(a => new ListingBuyerAdDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Budget = a.Budget,
                    CreateDate = a.CreateDate,
                    Deadline = a.Deadline,
                    Category = a.Category,
                    BuyerName = a.Buyer.UserName ?? string.Empty,
                    BuyerUserId = a.Buyer.Id,
                    IsClosed = a.IsClosed
                })
                .ToListAsync();

            if (ads.Count == 0)
                return NoContent();
            return Ok(ads);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("{id}/offers")]
        public async Task<IActionResult> SubmitOffer([FromRoute] int id, [FromBody] BuyerAdOfferDTO offerDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (seller == null) return BadRequest("User is not a registered freelancer.");

            var ad = await _context.BuyerAds.FindAsync(id);
            if (ad == null) return NotFound("Ad not found.");
            if (ad.IsClosed) return BadRequest("Ad is closed.");

            // Check if already submitted
            var existingOffer = await _context.BuyerAdOffers
                .FirstOrDefaultAsync(o => o.BuyerAdId == id && o.FreelancerId == seller.Id);
            if (existingOffer != null) return BadRequest("You have already submitted an offer for this ad.");

            var offer = new BuyerAdOffer
            {
                BuyerAdId = id,
                FreelancerId = seller.Id,
                Price = offerDto.Price,
                Deadline = offerDto.Deadline,
                Description = offerDto.Description,
                CreatedAt = DateTime.UtcNow,
                IsAccepted = false
            };

            _context.BuyerAdOffers.Add(offer);
            await _context.SaveChangesAsync();

            return Ok("Offer submitted successfully.");
        }

        [Authorize]
        [HttpGet("{id}/offers")]
        public async Task<IActionResult> GetOffers([FromRoute] int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var ad = await _context.BuyerAds.Include(a => a.Buyer).FirstOrDefaultAsync(a => a.Id == id);
            if (ad == null) return NotFound();

            // Buyer Isolation: Only owner can see all offers
            if (ad.Buyer.Id == userId)
            {
                var offers = await _context.BuyerAdOffers
                    .Where(o => o.BuyerAdId == id)
                    .Select(o => new {
                        o.Id,
                        o.Price,
                        o.Deadline,
                        o.Description,
                        o.CreatedAt,
                        o.IsAccepted,
                        FreelancerName = o.Freelancer.User.UserName,
                        FreelancerUserId = o.Freelancer.UserId
                    })
                    .ToListAsync();
                return Ok(offers);
            }

            // Blind Bidding: Freelancer can only see their own offer
            var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (seller != null)
            {
                var myOffer = await _context.BuyerAdOffers
                    .Where(o => o.BuyerAdId == id && o.FreelancerId == seller.Id)
                    .Select(o => new {
                        o.Id,
                        o.Price,
                        o.Deadline,
                        o.Description,
                        o.CreatedAt,
                        o.IsAccepted,
                        FreelancerName = o.Freelancer.User.UserName,
                        FreelancerUserId = o.Freelancer.UserId
                    })
                    .FirstOrDefaultAsync();
                
                if (myOffer != null) return Ok(new List<object> { myOffer });
            }

            return Forbid();
        }

        [Authorize(Roles = "Buyer")]
        [HttpPost("offers/{offerId}/accept")]
        public async Task<IActionResult> AcceptOffer([FromRoute] int offerId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var offer = await _context.BuyerAdOffers
                .Include(o => o.BuyerAd)
                .ThenInclude(a => a.Buyer)
                .Include(o => o.Freelancer)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (offer == null) return NotFound("Offer not found.");
            if (offer.BuyerAd.Buyer.Id != userId) return Forbid();
            if (offer.BuyerAd.IsClosed) return BadRequest("Ad is already closed.");

            // Transaction to ensure atomicity
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                offer.IsAccepted = true;
                offer.BuyerAd.IsClosed = true;
                offer.BuyerAd.AcceptedOfferId = offer.Id;

                // Dynamically create a SellerAd for this contract
                var sellerAd = new SellerAd
                {
                    Title = offer.BuyerAd.Title,
                    Description = offer.BuyerAd.Description,
                    Category = offer.BuyerAd.Category,
                    CreateDate = DateTime.UtcNow,
                    FreelancerId = offer.Freelancer.UserId,
                    IsPrivate = true
                };
                
                _context.SellerAds.Add(sellerAd);
                await _context.SaveChangesAsync();

                // Dynamically create a matching Gig mapping to the accepted offer
                var gig = new Gigs
                {
                    TierName = "Accepted Offer",
                    TierDescription = "Contract generated from Buyer Ad",
                    Price = offer.Price,
                    SellerAdId = sellerAd.Id
                };

                _context.Gigs.Add(gig);
                await _context.SaveChangesAsync();

                // Create Order
                var order = new Order
                {
                    ExtOrderId = Guid.NewGuid().ToString(),
                    Status = "Paid", // Automatically marked as paid/active
                    OrderDate = DateTime.UtcNow,
                    AproxDeliveryDate = offer.Deadline,
                    Price = (int)offer.Price,
                    Quantity = 1,
                    AdditionalInstructions = $"Contract for Buyer Ad: {offer.BuyerAd.Title}. \n\nAccepted Offer Description: {offer.Description}",
                    BuyerId = (await _context.Buyers.FirstAsync(b => b.UserId == userId)).Id,
                    SellerId = offer.FreelancerId,
                    GigsId = gig.Id 
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Offer accepted and order created.", OrderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
