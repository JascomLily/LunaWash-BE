using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class Equipment
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = null!;

        [StringLength(50)]
        public string BranchId { get; set; } = null!;

        [StringLength(150)]
        public string Name { get; set; } = null!;

        [StringLength(100)]
        public string Category { get; set; } = null!;

        [StringLength(50)]
        public string Status { get; set; } = "Hoạt động"; // Hoạt động, Cần kiểm tra, Đang bảo trì, Lỗi

        [StringLength(50)]
        public string Priority { get; set; } = "Bình thường"; // Bình thường, Trung bình, Cao, Khẩn cấp

        [StringLength(50)]
        public string? LastMaintenance { get; set; }

        [StringLength(50)]
        public string? NextMaintenance { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        public virtual ICollection<MaintenanceTask> MaintenanceTasks { get; set; } = new List<MaintenanceTask>();
    }
}
