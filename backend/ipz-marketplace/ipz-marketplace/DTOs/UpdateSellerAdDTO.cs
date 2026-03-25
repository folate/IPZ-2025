using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ipz_marketplace.DTOs
{
    public class UpdateSellerAdDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<GigsDTO> Gigs { get; set; }
        public IFormFileCollection? NewPhotos { get; set; }
        public int? MainPhotoId { get; set; }
    }
}
