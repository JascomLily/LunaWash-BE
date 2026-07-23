using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class MaintenanceTask
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = null!;

        [StringLength(50)]
        public string EquipmentId { get; set; } = null!;

        [StringLength(50)]
        public string BranchId { get; set; } = null!;

        [StringLength(200)]
        public string TaskName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Chưa làm"; // Chưa làm, Đang làm, Hoàn thành, Trễ hạn

        [StringLength(50)]
        public string Priority { get; set; } = "Medium"; // High, Medium, Low

        [StringLength(1000)]
        public string? ReviewNote { get; set; }

        [StringLength(50)]
        public string? AssigneeId { get; set; }

        [StringLength(50)]
        public string? IncidentReportId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; } = null!;
        
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        [ForeignKey("AssigneeId")]
        public virtual User? Assignee { get; set; }

        [ForeignKey("IncidentReportId")]
        public virtual IncidentReport? IncidentReport { get; set; }
    }
}
