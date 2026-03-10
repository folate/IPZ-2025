using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

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
            if (file == null || file.Length == 0) return BadRequest("Problem with sending file.");

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
                SellerAdId = AdId
            };

            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            return Ok("Photo added successfully!");
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetPhoto(int AdId)
        {
            var photos = _context.Photos
                .Where(p => p.SellerAdId == AdId)
                .Select(p => new
            {
                p.Id,
                p.Url,
                p.IsMain
            }).ToList();

            if (photos.Any()) 
                return Ok(photos);

            return BadRequest("No photos where found");
        }
    }
}
