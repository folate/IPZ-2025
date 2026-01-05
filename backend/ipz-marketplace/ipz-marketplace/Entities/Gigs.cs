using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ipz_marketplace.Entities;

public class Gigs
{
    public int Id { get; set; }
    public string TierName { get; set; }
    public string TierDescription { get; set; }
    public decimal Price { get; set; }

    public int SellerAdId { get; set; }
    public SellerAd SellerAd { get; set; }
}