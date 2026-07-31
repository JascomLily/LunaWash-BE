using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class StaffSchedule
    {
        [Key]
        public string Id { get; set; } = null!;

        public string EmployeeId { get; set; } = null!;

        public string Shift { get; set; } = "Ca sáng"; // "Ca sáng", "Ca chiều", "Ca tối"

        public string DayOff { get; set; } = "Thứ Hai"; // "Thứ Hai", "Thứ Ba",...

        public string BranchId { get; set; } = null!;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("EmployeeId")]
        public virtual User Employee { get; set; } = null!;

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;
    }
}
