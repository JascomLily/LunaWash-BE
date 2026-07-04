using System;
using System.ComponentModel.DataAnnotations;

namespace LunaWash.BLL.DTOs
{
    public class ReviewDto
    {
        public string Id { get; set; } = null!;
        public string BookingId { get; set; } = null!;
        public string? BranchId { get; set; }
        public double OverallRating { get; set; }
        public int CleanlinessRating { get; set; }
        public int SpeedRating { get; set; }
        public int StaffRating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ResponseText { get; set; }
        public string? RespondedById { get; set; }
        public string? RespondedByName { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? CustomerName { get; set; }
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

    public class ReviewRespondDto
    {
        [Required]
        [MaxLength(1000)]
        public string ResponseText { get; set; } = null!;
    }
}
