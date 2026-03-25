using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class UserModifyDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Login { get; set; }

        [MinLength(8)]
        public string? Password { get; set; }

        public bool isFreelancer { get; set; }
    }
}
