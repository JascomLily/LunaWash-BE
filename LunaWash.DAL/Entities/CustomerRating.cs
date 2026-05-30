using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class CustomerRating
{
    public string Id { get; set; } = null!;

    public string BookingId { get; set; } = null!;

    public int CooperationRating { get; set; }

    public string? StaffComment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
