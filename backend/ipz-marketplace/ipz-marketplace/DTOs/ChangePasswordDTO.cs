using System.ComponentModel.DataAnnotations;

namespace ipz_marketplace.DTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
