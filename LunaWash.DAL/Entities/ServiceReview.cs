using System;

namespace LunaWash.DAL.Entities;

public partial class ServiceReview
{
    public string Id { get; set; } = null!;

    public string BookingId { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public string BranchId { get; set; } = null!;

    public double OverallRating { get; set; }
    public int CleanlinessRating { get; set; }
    public int SpeedRating { get; set; }
    public int StaffRating { get; set; }

    public string? Comment { get; set; }
    
    public DateTime? CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;
    public virtual User Customer { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}
