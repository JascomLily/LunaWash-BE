using System;

namespace LunaWash.DAL.Entities;

public partial class Attendance
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public DateTime AttendanceDate { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public string Status { get; set; } = null!; // e.g. Present, Late, Absent, OnLeave

    public string? Note { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;
}
