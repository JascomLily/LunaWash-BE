namespace LunaWash.BLL.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Tier { get; set; } = null!;
        public string? BranchId { get; set; }
    }
}
