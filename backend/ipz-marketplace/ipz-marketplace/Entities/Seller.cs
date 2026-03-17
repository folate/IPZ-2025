using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ipz_marketplace.Entities;

public class Seller
{
    [Required]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }

    [MaxLength(1000)]
    public string Bio { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Skills { get; set; } = string.Empty;

    public decimal HourlyRate { get; set; } = 0;

    public int CompletedJobs { get; set; } = 0;

    public decimal Rating { get; set; } = 0;

    public int TotalReviews { get; set; } = 0;

    public DateTime JoinedDate { get; set; }

    public string PortfolioUrl { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;
}
