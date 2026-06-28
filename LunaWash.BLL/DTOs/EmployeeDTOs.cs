using System;

namespace LunaWash.BLL.DTOs
{
    public class EmployeeResponseDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string RoleId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string? BranchId { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeCreateDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string RoleId { get; set; } = null!; // "Staff", "TechnicalStaff"
        public string BranchId { get; set; } = null!;
    }

    public class AttendanceResponseDto
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string BranchId { get; set; } = null!;
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; } = null!; 
    }
}
