using System;
using System.Collections.Generic;

namespace LunaWash.BLL.DTOs
{
    public class UserBranchResponseDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public decimal? Salary { get; set; }
        public int LeaveDays { get; set; }
        public bool IsActive { get; set; }
    }

    public class DailyAttendanceResponseDto
    {
        public string EmployeeId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string Status { get; set; } = null!; // e.g. "Có mặt", "Vắng mặt", "Vắng có phép"
        public string? CheckInTime { get; set; }
        public string? Notes { get; set; }
    }

    public class AttendanceRecordDto
    {
        public string EmployeeId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Note { get; set; }
    }

    public class SaveAttendanceDto
    {
        public string BranchId { get; set; } = null!;
        public string Shift { get; set; } = null!;
        public List<AttendanceRecordDto> Attendances { get; set; } = new List<AttendanceRecordDto>();
    }

    public class ShiftTemplateResponseDto
    {
        public string Id { get; set; } = null!; // maps to employeeId in FE t.id
        public string Shift { get; set; } = null!;
        public string DayOff { get; set; } = null!;
    }

    public class ShiftTemplateItemDto
    {
        public string EmployeeId { get; set; } = null!;
        public string Shift { get; set; } = null!;
        public string DayOff { get; set; } = null!;
    }

    public class SaveShiftTemplatesDto
    {
        public List<ShiftTemplateItemDto> Templates { get; set; } = new List<ShiftTemplateItemDto>();
    }

    public class ScheduleHistoryResponseDto
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
