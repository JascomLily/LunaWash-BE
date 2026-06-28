using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class EquipmentDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string StatusColor { get; set; } = null!;
        public string StatusIcon { get; set; } = null!;
        public string? LastMaintenance { get; set; }
        public string? NextMaintenance { get; set; }
        public string? NextMaintenanceColor { get; set; }
        public string Priority { get; set; } = null!;
        public string PriorityColor { get; set; } = null!;
    }

    public class MaintenanceTaskDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string StatusColor { get; set; } = null!;
        public string? Description { get; set; }
        public string? EquipmentId { get; set; }
        public string? EquipmentName { get; set; }
    }

    public class EquipmentDashboardDTO
    {
        public List<EquipmentDTO> Equipments { get; set; } = new List<EquipmentDTO>();
        public List<MaintenanceTaskDTO> Tasks { get; set; } = new List<MaintenanceTaskDTO>();
    }

    public class CreateIncidentDTO
    {
        [Required]
        public string EquipmentId { get; set; } = null!;
        
        [Required]
        public string Priority { get; set; } = null!;
        
        [Required]
        public string Description { get; set; } = null!;
    }

    public class UpdateEquipmentStatusDTO
    {
        [Required]
        public string Status { get; set; } = null!;
    }

    public class UpdateEquipmentScheduleDTO
    {
        [Required]
        public string NextMaintenance { get; set; } = null!;
    }

    public class UpdateEquipmentPriorityDTO
    {
        [Required]
        public string Priority { get; set; } = null!;
    }

    public class CreateReportDTO
    {
        [Required]
        public string IssueName { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Status { get; set; } = null!;

        [Required]
        public string TaskStatus { get; set; } = null!;
    }

    public class UpdateTaskStatusDTO
    {
        [Required]
        public string Status { get; set; } = null!;
    }
}
