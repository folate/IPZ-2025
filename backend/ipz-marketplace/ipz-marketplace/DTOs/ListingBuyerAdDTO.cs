using ipz_marketplace.Entities;

namespace ipz_marketplace.DTOs
{
    public class ListingBuyerAdDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Budget { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Category { get; set; }
        public string BuyerName { get; set; }
    }
}
