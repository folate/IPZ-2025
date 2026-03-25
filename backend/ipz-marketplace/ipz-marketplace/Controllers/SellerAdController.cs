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
    public class SellerAdController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly MarketplaceDbContext _context;
        private readonly EmailService _emailService;
        public SellerAdController(UserManager<User> userManager, MarketplaceDbContext context, EmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAd([FromForm] SellerAdDTO adDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized("userId went wrong!");
            }

            var logged = await _userManager.FindByIdAsync(userId);

            if (logged == null)
            {
                return NotFound("user was not found!");
            }

            var photosFile = new List<AdPhoto>();

            if (adDto.Photos != null && adDto.Photos.Any())
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "photos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in adDto.Photos)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        var stream = new FileStream(filePath, FileMode.Create);

                        await file.CopyToAsync(stream);
                        var adPhoto = new AdPhoto
                        {
                            Url = $"/photos/{fileName}",
                            IsMain = (adDto.Photos.Count == 0)
                        };
                        photosFile.AddRange(adPhoto);
                    }
                }
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
                }).ToList(),
                Photos = photosFile
            };

            await _emailService.EmailConnection(logged.Email, "Sucessfuly created Ad!", $"Thank you {logged.UserName} for creating Ad for our service.");

            _context.SellerAds.Add(ad);
            await _context.SaveChangesAsync();

            return Ok("Sucessfuly created!");
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditAd([FromRoute] int id, [FromForm] UpdateSellerAdDTO adDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized("userId went wrong!");

            var ad = await _context.SellerAds
                .Include(a => a.Gigs)
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null) return NotFound("Ad not found");
            if (ad.FreelancerId != userId) return Forbid("You do not own this ad");

            ad.Title = adDto.Title ?? ad.Title;
            ad.Description = adDto.Description ?? ad.Description;
            if (!string.IsNullOrEmpty(adDto.Category))
            {
                ad.Category = adDto.Category;
            }

            if (adDto.Gigs != null && adDto.Gigs.Any())
            {
                _context.RemoveRange(ad.Gigs);
                ad.Gigs = adDto.Gigs.Select(g => new Gigs
                {
                    TierName = g.TierName,
                    TierDescription = g.TierDescription,
                    Price = g.Price
                }).ToList();
            }

            if (adDto.NewPhotos != null && adDto.NewPhotos.Any())
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "photos");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                foreach (var file in adDto.NewPhotos)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        ad.Photos.Add(new AdPhoto
                        {
                            Url = $"/photos/{fileName}",
                            IsMain = false
                        });
                    }
                }
            }

            if (adDto.MainPhotoId.HasValue)
            {
                bool found = false;
                foreach (var photo in ad.Photos)
                {
                    if (photo.Id == adDto.MainPhotoId.Value)
                    {
                        photo.IsMain = true;
                        found = true;
                    }
                    else
                    {
                        photo.IsMain = false;
                    }
                }
                
                // fallback if provided ID was not found or 0
                if (!found && ad.Photos.Any())
                {
                    ad.Photos.First().IsMain = true;
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Successfully updated ad!");
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
                    Freelancer = a.Freelancer != null ? a.Freelancer.UserName : "",
                    FreelancerId = a.FreelancerId,
                    SellerId = _context.Sellers.Where(s => s.UserId == a.FreelancerId).Select(s => s.Id).Cast<int?>().FirstOrDefault(),
                    Category = a.Category,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        Id = g.Id,
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList(),
                    Photos = a.Photos.Select(p => new AdPhoto
                    { 
                        Url = p.Url,
                        IsMain = p.IsMain
                    }).ToList()
                })
                .FirstOrDefault(a => a.Id == id);
            return Ok(ad);
        }
        [HttpGet("all/{number}")]
        public async Task<IActionResult> GetFewAds([FromRoute] int number)
        {
            List<ListingSellerAdDTO> sellerAds = _context.SellerAds
                .Where(a => !a.IsPrivate)
                .Select(a => new ListingSellerAdDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Freelancer = a.Freelancer != null ? a.Freelancer.UserName : "",
                    FreelancerId = a.FreelancerId,
                    SellerId = _context.Sellers.Where(s => s.UserId == a.FreelancerId).Select(s => s.Id).Cast<int?>().FirstOrDefault(),
                    Category = a.Category,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        Id = g.Id,
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList(),
                    Photos = a.Photos.Select(p => new AdPhoto
                    {
                        Url = p.Url,
                        IsMain= p.IsMain
                    }).ToList()
                })
                .Take(number)
                .OrderBy(a => a.Id)
                .ToList();

            return Ok(sellerAds);
        }

        [AllowAnonymous]
        [HttpGet("freelancer/{freelancerId}")]
        public async Task<IActionResult> GetAdsByFreelancer([FromRoute] string freelancerId)
        {
            List<ListingSellerAdDTO> sellerAds = _context.SellerAds
                .Where(a => a.FreelancerId == freelancerId && !a.IsPrivate)
                .Select(a => new ListingSellerAdDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Freelancer = a.Freelancer != null ? a.Freelancer.UserName : "",
                    FreelancerId = a.FreelancerId,
                    SellerId = _context.Sellers.Where(s => s.UserId == a.FreelancerId).Select(s => s.Id).Cast<int?>().FirstOrDefault(),
                    Category = a.Category,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList(),
                    Photos = a.Photos.Select(p => new AdPhoto
                    {
                        Url = p.Url,
                        IsMain = p.IsMain
                    }).ToList()
                })
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
                .Where(a => a.FreelancerId == userId && !a.IsPrivate)
                .Select(a => new ListingSellerAdDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Freelancer = a.Freelancer != null ? a.Freelancer.UserName : "",
                    FreelancerId = userId,
                    SellerId = _context.Sellers.Where(s => s.UserId == userId).Select(s => s.Id).Cast<int?>().FirstOrDefault(),
                    Category = a.Category,
                    Gigs = a.Gigs.Select(g => new GigsDTO
                    {
                        TierName = g.TierName,
                        TierDescription = g.TierDescription,
                        Price = g.Price
                    }).ToList(),
                    Photos = a.Photos.Select(p => new AdPhoto
                    {
                        Url = p.Url,
                        IsMain = p.IsMain
                    }).ToList()
                })
                .ToList();
            return Ok(sellerAds);
        }
    }
}
