using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class CreateMaintenanceRequest
    {
        [Required]
        public string EquipmentId { get; set; } = null!;

        [Required]
        public string BranchId { get; set; } = null!;

        [Required]
        public string TaskName { get; set; } = null!;

        public string? Description { get; set; }

        public string Priority { get; set; } = "Medium";

        public string? AssigneeId { get; set; }

        public string? IncidentReportId { get; set; }
    }

    public class MaintenanceResponse
    {
        public string Id { get; set; } = null!;
        public string EquipmentId { get; set; } = null!;
        public string EquipmentName { get; set; } = null!;
        public string BranchId { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string TaskName { get; set; } = null!;
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;
        public string? ReviewNote { get; set; }
        public string? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
        public string? IncidentReportId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateMaintenanceStatusRequest
    {
        [Required]
        public string Status { get; set; } = null!;
        
        public string? ReviewNote { get; set; }
    }
}
