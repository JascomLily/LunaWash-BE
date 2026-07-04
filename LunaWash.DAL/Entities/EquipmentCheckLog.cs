using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class EquipmentCheckLog
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = null!;

        [StringLength(50)]
        public string BranchId { get; set; } = null!;

        [StringLength(50)]
        public string EquipmentId { get; set; } = null!;

        [StringLength(50)]
        public string TechnicianId { get; set; } = null!;

        public DateTime CheckTime { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Condition { get; set; } = "Hoạt động tốt"; // Hoạt động tốt, Cần kiểm tra, Lỗi

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; } = null!;

        [ForeignKey("TechnicianId")]
        public virtual User Technician { get; set; } = null!;
    }
}
