using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ipz_marketplace.Entities;

public class Buyer
{
    [Required]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public User User { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;
    public string BillingAddress { get; set; } = string.Empty;

    public int TotalOrders { get; set; } = 0;

    public DateTime JoinedDate { get; set; }
    public DateTime? LastOrderDate { get; set; } = null;

    public string PreferredPaymentMethod { get; set; } = string.Empty;
}
