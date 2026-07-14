using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class ReviewDto
    {
        public string Id { get; set; } = null!;
        public string BookingId { get; set; } = null!;
        public double OverallRating { get; set; }
        public int CleanlinessRating { get; set; }
        public int SpeedRating { get; set; }
        public int StaffRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CustomerName { get; set; }
        public string? VehicleInfo { get; set; }
        public string? Reply { get; set; }
    }

    public class CreateReviewDto
    {
        [Required]
        public string BookingId { get; set; } = null!;
        
        [Range(1.0, 5.0)]
        public double ServiceRating { get; set; } // Map to OverallRating in Backend
        
        [Range(1, 5)]
        public int CleanlinessRating { get; set; }
        
        [Range(1, 5)]
        public int SpeedRating { get; set; }
        
        [Range(1, 5)]
        public int StaffRating { get; set; }
        
        [MaxLength(1000)]
        public string? Comment { get; set; }
    }

    public class UpdateReviewDto
    {
        [Range(1.0, 5.0)]
        public double ServiceRating { get; set; }
        
        [Range(1, 5)]
        public int CleanlinessRating { get; set; }
        
        [Range(1, 5)]
        public int SpeedRating { get; set; }
        
        [Range(1, 5)]
        public int StaffRating { get; set; }
        
        [MaxLength(1000)]
        public string? Comment { get; set; }
    }

    public class ReplyReviewRequestDto
    {
        [Required]
        [MaxLength(2000)]
        public string ReplyText { get; set; } = null!;
    }
}
