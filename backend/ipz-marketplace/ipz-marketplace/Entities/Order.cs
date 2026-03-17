using System.ComponentModel.DataAnnotations.Schema;

namespace ipz_marketplace.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime AproxDeliveryDate { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string? AdditionalInstructions { get; set; }

        [ForeignKey(nameof(SellerId))]
        public Seller Seller { get; set; }
        public int SellerId { get; set; }

        [ForeignKey(nameof(BuyerId))]
        public Buyer Buyer { get; set; }
        public int BuyerId { get; set; }

        [ForeignKey(nameof(GigsId))]
        public Gigs Gigs { get; set; }
        public int GigsId { get; set; }
    }
}
