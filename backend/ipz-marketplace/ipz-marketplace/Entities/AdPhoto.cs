namespace ipz_marketplace.Entities
{
    public class AdPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }

        public SellerAd SellerAd { get; set; }
        public int SellerAdId { get; set; }
    }
}
