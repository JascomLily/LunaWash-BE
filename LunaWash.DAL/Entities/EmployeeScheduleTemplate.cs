using System;

namespace LunaWash.DAL.Entities
{
    public class EmployeeScheduleTemplate
    {
        public string Id { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public string Shift { get; set; } = null!; // e.g. "Ca sáng", "Ca chiều"
        public string DayOff { get; set; } = null!; // e.g. "Thứ Hai"
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual User Employee { get; set; } = null!;
    }
}
