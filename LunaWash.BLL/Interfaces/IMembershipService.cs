using System.Collections.Generic;
using System.Threading.Tasks;

namespace LunaWash.BLL.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipTierDto>> GetAllTiersAsync();
        Task<bool> UpdateTierAsync(string id, MembershipTierUpdateDto dto);
        Task<bool> SyncCustomerTiersAsync();
    }

    public class MembershipTierDto
    {
        public string Id { get; set; } = null!;
        public string TierName { get; set; } = null!;
        public int MinPoints { get; set; }
        public int MinMaintainPoints { get; set; }
        public decimal PointsMultiplier { get; set; }
        public int PriorityLevel { get; set; }
        public decimal DiscountPercent { get; set; }
        public int CustomerCount { get; set; }
        public int MaxBookingDays { get; set; }
    }

    public class MembershipTierUpdateDto
    {
        public int MinPoints { get; set; }
        public int MinMaintainPoints { get; set; }
        public decimal PointsMultiplier { get; set; }
        public int PriorityLevel { get; set; }
        public decimal DiscountPercent { get; set; }
        public int MaxBookingDays { get; set; }
    }
}
