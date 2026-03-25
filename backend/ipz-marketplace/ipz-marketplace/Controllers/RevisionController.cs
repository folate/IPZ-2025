using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevisionController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;
        private readonly UserManager<User> _userManager;

        public RevisionController(MarketplaceDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Buyer")]
        [HttpPut("create")]
        public async Task<IActionResult> RequestRevision([FromForm] RevisionAddDTO revisionDTO)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .FirstOrDefaultAsync(o => o.Id == revisionDTO.OrderId);

            if (order == null)
            {
                return NotFound("Order not found");
            }

            if (order.Buyer.UserId != userId)
            {
                return Forbid("You are not the buyer of this order");
            }

            var revision = new Revision
            {
                OrderId = revisionDTO.OrderId,
                Reason = revisionDTO.Reason,
                Order = order,
                RequestDate = DateTime.UtcNow,
                Status = order.Status == "Delivered" ? "Pending" : "Update",
                SenderId = userId
            };

            if (revisionDTO.Files != null && revisionDTO.Files.Any())
            {
                var files = await SaveFiles(revisionDTO.Files);
                foreach (var file in files)
                {
                    revision.Files.Add(file);
                }
            }

            _context.Revisions.Add(revision);
            order.Status = "RevisionRequested";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok("Revision requested successfully");
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateRevisionStatus([FromForm] RevisionUpdateDTO revisionDTO)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.Id == revisionDTO.OrderId);

            if (order == null) return NotFound("Order not found");

            // Only buyer or seller of the order can update
            if (order.Buyer.UserId != userId && order.Seller.UserId != userId)
            {
                return Forbid();
            }

            // Always create a NEW revision entry to avoid overriding previous messages/files
            var revision = new Revision
            {
                OrderId = revisionDTO.OrderId,
                Reason = revisionDTO.Reason ?? $"Status updated to {revisionDTO.Status}",
                RequestDate = DateTime.UtcNow,
                Status = revisionDTO.Status,
                SenderId = userId
            };

            if (revisionDTO.Status == "Delivered")
            {
                order.Status = "Delivered";
            }
            else if (revisionDTO.Status == "Completed")
            {
                order.Status = "Completed";
                // If the seller has a CompletedJobs counter, update it here
                if (order.Seller != null)
                {
                    order.Seller.CompletedJobs += 1;
                    _context.Sellers.Update(order.Seller);
                }
            }
            else if (revisionDTO.Status == "Pending")
            {
                order.Status = "RevisionRequested";
            }

            if (revisionDTO.Files != null && revisionDTO.Files.Any())
            {
                var files = await SaveFiles(revisionDTO.Files);
                foreach (var file in files)
                {
                    revision.Files.Add(file);
                }
            }

            _context.Revisions.Add(revision);
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return Ok("Revision updated successfully");
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetRevisionsByOrderId([FromQuery] int orderId)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound("Order not found");

            if (order.Buyer.UserId != userId && order.Seller.UserId != userId)
            {
                return Forbid();
            }

            var revisions = await _context.Revisions
                .Include(r => r.Files)
                .Where(r => r.OrderId == orderId)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();

            var result = revisions.Select(r => new
            {
                r.Id,
                r.Reason,
                r.RequestDate,
                r.Status,
                r.SenderId,
                Files = r.Files.Select(f => new { f.Id, f.FileName })
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var userId = _userManager.GetUserId(User);
            var file = await _context.RevisionFiles
                .Include(f => f.Revision)
                    .ThenInclude(r => r.Order)
                        .ThenInclude(o => o.Buyer)
                .Include(f => f.Revision)
                    .ThenInclude(r => r.Order)
                        .ThenInclude(o => o.Seller)
                .FirstOrDefaultAsync(f => f.Id == fileId);

            if (file == null) return NotFound();

            if (file.Revision.Order.Buyer.UserId != userId && file.Revision.Order.Seller.UserId != userId)
            {
                return Forbid();
            }

            if (!System.IO.File.Exists(file.FilePath))
            {
                return NotFound("File not found on server");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(file.FilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(file.FileName), file.FileName);
        }

        private async Task<List<RevisionFile>> SaveFiles(IFormFileCollection files)
        {
            var revisionFiles = new List<RevisionFile>();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "revisions");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var storedName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadsFolder, storedName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    revisionFiles.Add(new RevisionFile
                    {
                        FileName = file.FileName,
                        StoredName = storedName,
                        FilePath = filePath
                    });
                }
            }
            return revisionFiles;
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip", "application/zip"}
            };
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}
