using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ipz_marketplace.DTOs
{
    public class RevisionAddDTO
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
        public IFormFileCollection? Files { get; set; }
    }
}
