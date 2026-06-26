using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class Booking
{
    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string VehicleTypeId { get; set; } = null!;

    public DateOnly BookingDate { get; set; }

    public DateTime ScheduledStartTime { get; set; }

    public DateTime ScheduledEndTime { get; set; }

    public string Status { get; set; } = null!;
    
    public decimal TotalPrice { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckoutTime { get; set; }

    public int PriorityScore { get; set; }

    public string? WashSlotId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;



    public virtual VehicleType VehicleType { get; set; } = null!;

    public virtual WashSlot? WashSlot { get; set; }


}
