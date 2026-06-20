using System;

namespace LunaWash.BLL.DTOs;

public class CreateReviewDto
{
    public string BookingId { get; set; } = null!;
    public double OverallRating { get; set; }
    public int CleanlinessRating { get; set; }
    public int SpeedRating { get; set; }
    public int StaffRating { get; set; }
    public string? Comment { get; set; }
}

public class ReviewResponseDto
{
    public string Id { get; set; } = null!;
    public string BookingId { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string BranchId { get; set; } = null!;
    
    public double OverallRating { get; set; }
    public int CleanlinessRating { get; set; }
    public int SpeedRating { get; set; }
    public int StaffRating { get; set; }
    
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
}
