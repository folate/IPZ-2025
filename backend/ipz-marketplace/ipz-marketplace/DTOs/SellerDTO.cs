namespace ipz_marketplace.DTOs
{
    public class SellerDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Skills { get; set; }
        public decimal HourlyRate { get; set; }
        public int CompletedJobs { get; set; }
        public decimal Rating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime JoinedDate { get; set; }
        public string PortfolioUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
