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
        public string Status { get; set; } = "Chưa làm"; // Chưa làm, Đang làm, Hoàn thành, Trễ hạn, Đã nghiệm thu

        [StringLength(50)]
        public string? AssignedToId { get; set; }

        [StringLength(500)]
        public string? Resolution { get; set; }

        [StringLength(500)]
        public string? SupportRequest { get; set; }

        public bool IsIncident { get; set; } = false;

        [StringLength(50)]
        public string? Priority { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; } = null!;
        
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        [ForeignKey("AssignedToId")]
        public virtual User? AssignedTo { get; set; }
    }
}
