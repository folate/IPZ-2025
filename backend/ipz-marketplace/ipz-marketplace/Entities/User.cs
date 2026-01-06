using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ipz_marketplace.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreateDate { get; set; }
    public string Email { get; set; }
    public bool IsFreelancer { get; set; }
}