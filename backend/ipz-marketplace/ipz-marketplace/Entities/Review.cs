using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ipz_marketplace.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SellerId { get; set; }
    public Seller Seller { get; set; }

    [Required]
    public int BuyerId { get; set; }
    public Buyer Buyer { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
