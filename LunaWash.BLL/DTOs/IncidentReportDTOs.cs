using System;

namespace LunaWash.BLL.DTOs
{
    public class IncidentReportCreateDto
    {
        public string EquipmentId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class IncidentReportResponseDto
    {
        public string Id { get; set; } = null!;
        public string BranchId { get; set; } = null!;
        public string EquipmentId { get; set; } = null!;
        public string EquipmentName { get; set; } = null!;
        public string ReporterId { get; set; } = null!;
        public string ReporterName { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
