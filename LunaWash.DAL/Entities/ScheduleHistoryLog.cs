using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class ScheduleHistoryLog
    {
        [Key]
        public string Id { get; set; } = null!;

        public string BranchId { get; set; } = null!;

        public string EmployeeId { get; set; } = null!;

        public string ModifiedById { get; set; } = null!;

        public string Action { get; set; } = null!; // e.g. "Cập nhật ca làm", "Cập nhật ngày nghỉ"

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("EmployeeId")]
        public virtual User Employee { get; set; } = null!;

        [ForeignKey("ModifiedById")]
        public virtual User ModifiedBy { get; set; } = null!;

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;
    }
}
