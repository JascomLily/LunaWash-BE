using System;
using System.Collections.Generic;

namespace LunaWash.BLL.DTOs
{
    public class StaffScheduleDto
    {
        public string? Id { get; set; }
        public string EmployeeId { get; set; } = null!;
        public string? FullName { get; set; }
        public string Shift { get; set; } = "Ca sáng";
        public string DayOff { get; set; } = "Thứ Hai";
    }

    public class SaveStaffSchedulesRequest
    {
        public List<StaffScheduleDto> Templates { get; set; } = new();
    }

    public class AttendanceEntryDto
    {
        public string EmployeeId { get; set; } = null!;
        public string Status { get; set; } = null!; // Present, Late, Absent, OnLeave
        public string? Note { get; set; }
    }

    public class SaveAttendanceRequest
    {
        public string BranchId { get; set; } = null!;
        public string Shift { get; set; } = null!;
        public List<AttendanceEntryDto> Attendances { get; set; } = new();
    }

    public class ScheduleHistoryLogDto
    {
        public string Id { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string ModifiedByFullName { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string EmployeeFullName { get; set; } = null!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
