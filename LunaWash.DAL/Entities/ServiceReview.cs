using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class ServiceReview
{
    public string Id { get; set; } = null!;

    public string BookingId { get; set; } = null!;

    public int ServiceRating { get; set; }

    public int StaffRating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
