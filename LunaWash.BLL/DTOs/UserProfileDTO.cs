namespace LunaWash.BLL.DTOs
{
    public class UserProfileResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Address { get; set; }
        public LoyaltyInfoDTO? Loyalty { get; set; }
    }

    public class LoyaltyInfoDTO
    {
        public int CurrentPoints { get; set; }
        public int AccumulatedPoints { get; set; }
        public string TierName { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
    }
}