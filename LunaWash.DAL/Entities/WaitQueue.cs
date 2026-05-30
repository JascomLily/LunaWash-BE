using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class WaitQueue
{
    public string Id { get; set; } = null!;

    public string BookingId { get; set; } = null!;

    public int QueuePosition { get; set; }

    public DateTime JoinedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
