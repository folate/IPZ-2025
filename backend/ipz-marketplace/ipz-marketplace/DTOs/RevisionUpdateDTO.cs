using Microsoft.AspNetCore.Http;

namespace ipz_marketplace.DTOs
{
    public class RevisionUpdateDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public string? Reason { get; set; }
        public IFormFileCollection? Files { get; set; }
    }
}
