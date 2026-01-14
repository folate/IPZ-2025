using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class UpdateSellerDTO
    {
        [MaxLength(1000)]
        public string Bio { get; set; }

        [MaxLength(500)]
        public string Skills { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? HourlyRate { get; set; }

        public string PortfolioUrl { get; set; }

        public bool? IsAvailable { get; set; }
    }
}
