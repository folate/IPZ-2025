using ipz_marketplace.DTOs;
using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ipz_marketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevisionController : ControllerBase
    {
        private readonly MarketplaceDbContext _context;
        RevisionController(MarketplaceDbContext context)
        {
            _context = context;
        }

        [Authorize("Buyer")]
        [HttpPut("create")]
        public IActionResult RequestRevision([FromBody] RevisionAddDTO revisionDTO)
        {
            var order = _context.Orders.Find(revisionDTO.OrderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            var revision = new Revision
            {
                OrderId = revisionDTO.OrderId,
                Reason = revisionDTO.Reason,
                Order = order,
                RequestDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Revisions.Add(revision);
            _context.SaveChanges();
            return Ok("Revision requested successfully");
        }

        [HttpPost("update")]
        public IActionResult UpdateRevisionStatus([FromBody] RevisionUpdateDTO revisionDTO)
        {
            var revision = _context.Revisions.Find(revisionDTO.OrderId);
            if (revision == null)
            {
                return NotFound("Revision not found");
            }
            revision.Status = revisionDTO.Status;
            _context.SaveChanges();
            return Ok("Revision updated successfully");
        }

        [HttpGet("get")]
        public IActionResult GetRevisionsByOrderId([FromRoute]int orderId)
        {
            var revisions = _context.Revisions.Where(r => r.OrderId == orderId).ToList();
            if (revisions == null || revisions.Count == 0)
            {
                return NotFound("No revisions found");
            }
            return Ok(revisions);
        }
    }
}
