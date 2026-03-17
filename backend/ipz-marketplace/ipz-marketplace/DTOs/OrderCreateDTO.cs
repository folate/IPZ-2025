namespace ipz_marketplace.DTOs
{
    public class OrderCreateDTO
    {
        public int Quantity { get; set; }
        public string? AdditionalInstructions { get; set; }
        public DateTime AproxDeliveryTime { get; set; }
        public int GigId { get; set; }
    }
}
