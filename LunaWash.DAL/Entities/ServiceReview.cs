using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities
{
    public class ServiceReview
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string BookingId { get; set; } = null!;

        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string CustomerId { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string BranchId { get; set; } = null!;

        public double OverallRating { get; set; }

        public int CleanlinessRating { get; set; }

        public int SpeedRating { get; set; }

        public int StaffRating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
