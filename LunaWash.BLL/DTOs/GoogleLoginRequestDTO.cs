using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class GoogleLoginRequestDTO
    {
        [Required]
        public string Token { get; set; } = null!;
    }
}
