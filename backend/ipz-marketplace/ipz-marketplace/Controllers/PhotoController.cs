using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;
        public PhotoController(MarketplaceDbContext context)
        {
            _context = context;
        }

        [HttpPut("upload")]
        public async Task<IActionResult> AddPhoto(int AdId, IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "photos");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var photo = new AdPhoto
            {
                Url = $"/photos/{fileName}",
                IsMain = true,
                SellerAdId = AdId,
                SellerAd = _context.SellerAds.FirstOrDefault(ad => ad.Id == AdId)
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            return Ok("Photo added successfully");
        }
    }
}
