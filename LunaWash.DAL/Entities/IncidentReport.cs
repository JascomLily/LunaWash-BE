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

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(50)]
        public string? EquipmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string BranchId { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string ReporterId { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Mới báo cáo"; // Mới báo cáo, Đang xử lý, Đã giải quyết, Từ chối

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("EquipmentId")]
        public virtual Equipment? Equipment { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        [ForeignKey("ReporterId")]
        public virtual User Reporter { get; set; } = null!;
    }
}
