using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class CreateIncidentRequest
    {
        [Required]
        public string Title { get; set; } = null!;

        public string? EquipmentId { get; set; }

        [Required]
        public string BranchId { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }

    public class IncidentResponse
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
        public string BranchId { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string ReporterId { get; set; } = null!;
        public string ReporterName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateIncidentStatusRequest
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
