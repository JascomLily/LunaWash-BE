using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class UpdateProfileDTO
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;
    }
}