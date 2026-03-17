using ipz_marketplace.Entities;
using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class SellerAdDTO
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<GigsDTO> Gigs { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}
