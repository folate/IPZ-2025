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

        public Seller Seller { get; set; }
        [ForeignKey(nameof(Seller))]
        public int SellerId { get; set; }

        public Buyer Buyer { get; set; }
        [ForeignKey(nameof(Buyer))]
        public int BuyerId { get; set; }

        public Gigs Gigs { get; set; }
        [ForeignKey(nameof(Gigs))]
        public int GigsId { get; set; }
    }
}
