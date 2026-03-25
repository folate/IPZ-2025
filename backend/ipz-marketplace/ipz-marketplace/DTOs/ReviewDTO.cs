using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs;

public class CreateReviewDTO
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
}

public class ReviewDTO
{
    public int Id { get; set; }
    public string BuyerName { get; set; }
    public int Rating { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
