namespace ipz_marketplace.DTOs
{
    public class UpdateBuyerDTO
    {
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string PreferredPaymentMethod { get; set; }
    }
}
