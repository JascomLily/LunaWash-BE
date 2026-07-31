using System;

namespace LunaWash.BLL.DTOs
{
    public class EquipmentCheckLogCreateDto
    {
        public string EquipmentId { get; set; } = null!;
        public string Condition { get; set; } = null!; // Hoạt động tốt, Cần kiểm tra, Lỗi
        public string? Notes { get; set; }
    }

    public class EquipmentCheckLogResponseDto
    {
        public string Id { get; set; } = null!;
        public string BranchId { get; set; } = null!;
        public string EquipmentId { get; set; } = null!;
        public string EquipmentName { get; set; } = null!;
        public string TechnicianId { get; set; } = null!;
        public string TechnicianName { get; set; } = null!;
        public DateTime CheckTime { get; set; }
        public string Condition { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
