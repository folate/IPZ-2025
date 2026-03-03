using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.Entities;

public class BuyerAd
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime CreateDate { get; set; }
    public DateTime Deadline { get; set; }
    public string Category { get; set; }
    public int Budget { get; set; }
    public User Buyer { get; set; }
    public List<User> UsersBidding { get; set; } = new List<User>();

}