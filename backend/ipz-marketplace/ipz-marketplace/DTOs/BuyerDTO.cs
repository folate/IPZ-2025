using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class BuyerDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public int TotalOrders { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public string PreferredPaymentMethod { get; set; }
    }
}
