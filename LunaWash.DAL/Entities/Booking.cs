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

    public DateTime? CheckInTime { get; set; }

    public int PriorityScore { get; set; }

    public string? WashSlotId { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual CustomerRating? CustomerRating { get; set; }

    public virtual ICollection<CustomerVoucher> CustomerVouchers { get; set; } = new List<CustomerVoucher>();

    public virtual Invoice? Invoice { get; set; }

    public virtual ServiceExecutionLog? ServiceExecutionLog { get; set; }

    public virtual ServiceReview? ServiceReview { get; set; }

    public virtual VehicleType VehicleType { get; set; } = null!;

    public virtual ICollection<WaitQueue> WaitQueues { get; set; } = new List<WaitQueue>();

    public virtual WashSlot? WashSlot { get; set; }

    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();
}
