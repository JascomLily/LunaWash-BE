using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class RegisterRequestDTO
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        [StringLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
