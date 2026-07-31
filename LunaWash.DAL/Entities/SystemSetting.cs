using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.DAL.Entities
{
    public class SystemSetting
    {
        [Key]
        [StringLength(100)]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Value { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
