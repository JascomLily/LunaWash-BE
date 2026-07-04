using System;
using System.Collections.Generic;

namespace LunaWash.BLL.DTOs
{
    public class MembershipTierCreateUpdateDto
    {
        public string TierName { get; set; } = null!;
        public int MinPoints { get; set; }
        public decimal PointsMultiplier { get; set; }
        public int PriorityLevel { get; set; }
        public decimal DiscountPercent { get; set; }
        public int KeepPoints { get; set; }
        public int AdvanceBookingDays { get; set; }
    }

    public class MembershipTierResponseDto
    {
        public string Id { get; set; } = null!;
        public string TierName { get; set; } = null!;
        public int MinPoints { get; set; }
        public decimal PointsMultiplier { get; set; }
        public int PriorityLevel { get; set; }
        public decimal DiscountPercent { get; set; }
        public int KeepPoints { get; set; }
        public int AdvanceBookingDays { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdjustPointsDto
    {
        public string UserId { get; set; } = null!;
        public int PointsChange { get; set; } // Có thể âm (trừ điểm) hoặc dương (cộng điểm)
        public string Description { get; set; } = null!;
    }

    public class PointHistoryResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int Points { get; set; }
        public int RemainingPoints { get; set; }
        public string Description { get; set; } = null!;
        public string? BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
