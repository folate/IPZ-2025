using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.Entities;

public class SellerAd
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string FreelancerId { get; set; }
    public User Freelancer { get; set; }
    public ICollection<Gigs> Gigs { get; set; } = new List<Gigs>();
}