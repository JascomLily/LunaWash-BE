using System;

namespace LunaWash.DAL.Entities
{
    public class StaffScheduleHistory
    {
        public string Id { get; set; } = null!;
        public string BranchId { get; set; } = null!;
        public string ModifiedById { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public string Action { get; set; } = null!; // e.g. "Cập nhật ca trực"
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Branch Branch { get; set; } = null!;
        public virtual User ModifiedBy { get; set; } = null!;
        public virtual User Employee { get; set; } = null!;
    }
}
