using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = null!;

        [StringLength(250)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
