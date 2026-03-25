using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ipz_marketplace.Entities;

public class BuyerAdOffer
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int BuyerAdId { get; set; }
    [ForeignKey(nameof(BuyerAdId))]
    public BuyerAd BuyerAd { get; set; }

    [Required]
    public int FreelancerId { get; set; }
    [ForeignKey(nameof(FreelancerId))]
    public Seller Freelancer { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public DateTime Deadline { get; set; }

    [Required]
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsAccepted { get; set; } = false;
}
