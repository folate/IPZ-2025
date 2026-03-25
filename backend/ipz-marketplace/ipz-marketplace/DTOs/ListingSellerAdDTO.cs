using ipz_marketplace.Entities;
using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class ListingSellerAdDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string FreelancerId { get; set; }
        public int? SellerId { get; set; }
        public string Freelancer { get; set; }
        public List<GigsDTO> Gigs { get; set; }
        public List<AdPhoto> Photos { get; set; }
    }
}
