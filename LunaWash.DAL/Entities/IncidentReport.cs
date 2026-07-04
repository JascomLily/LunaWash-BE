using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class IncidentReport
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = null!;

        [StringLength(50)]
        public string BranchId { get; set; } = null!;

        [StringLength(50)]
        public string EquipmentId { get; set; } = null!;

        [StringLength(50)]
        public string ReporterId { get; set; } = null!;

        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Chờ duyệt"; // Chờ duyệt, Đã duyệt, Đã từ chối

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; } = null!;

        [ForeignKey("ReporterId")]
        public virtual User Reporter { get; set; } = null!;
    }
}
