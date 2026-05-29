using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(250)]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string RoleId { get; set; } = null!;

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation property
        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;
    }
}
