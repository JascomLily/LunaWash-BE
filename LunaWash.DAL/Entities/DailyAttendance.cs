using System;

namespace LunaWash.DAL.Entities
{
    public class DailyAttendance
    {
        public string Id { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
        public DateTime Date { get; set; } // The date portion
        public string Shift { get; set; } = null!; // e.g. "Ca sáng", "Ca chiều"
        public string Status { get; set; } = null!; // e.g. "Có mặt", "Vắng mặt"
        public DateTime? CheckInTime { get; set; } 
        public string? Notes { get; set; }

        public virtual User Employee { get; set; } = null!;
    }
}
