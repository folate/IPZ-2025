using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class CreateSellerDTO
    {
        [MaxLength(1000)]
        public string Bio { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Skills { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal HourlyRate { get; set; } = 0;

        public string PortfolioUrl { get; set; } = string.Empty;
    }
}
