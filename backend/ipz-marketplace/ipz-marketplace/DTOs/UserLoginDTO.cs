using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
        public bool doNotLogout { get; set; }
    }
}
