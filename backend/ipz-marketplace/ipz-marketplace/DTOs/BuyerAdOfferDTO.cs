using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs;

public class BuyerAdOfferDTO
{
    [Required]
    public decimal Price { get; set; }

    [Required]
    public DateTime Deadline { get; set; }

    [Required]
    public string Description { get; set; }
}
