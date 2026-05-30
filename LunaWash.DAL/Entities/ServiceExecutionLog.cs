using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class ServiceExecutionLog
{
    public string Id { get; set; } = null!;

    public string BookingId { get; set; } = null!;

    public DateTime ActualStartTime { get; set; }

    public DateTime ActualEndTime { get; set; }

    public int ActualDurationMinutes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
