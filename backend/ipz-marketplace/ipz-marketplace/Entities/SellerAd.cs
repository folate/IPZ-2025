using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ipz_marketplace.Entities;

public class SellerAd
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string Category { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime CreateDate { get; set; }
    public string FreelancerId { get; set; }
    public User Freelancer { get; set; }
    [Required]
    [JsonIgnore]
    public ICollection<Gigs> Gigs { get; set; } = new List<Gigs>();
    public ICollection<AdPhoto> Photos { get; set; } = new List<AdPhoto>();
    public bool IsPrivate { get; set; } = false;
}