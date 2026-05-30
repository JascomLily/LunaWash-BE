using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class CustomerVoucher
{
    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public string VoucherId { get; set; } = null!;

    public DateTime RedeemedDate { get; set; }

    public bool IsUsed { get; set; }

    public DateTime? UsedAt { get; set; }

    public string? UsedAtBookingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual Booking? UsedAtBooking { get; set; }

    public virtual Voucher Voucher { get; set; } = null!;
}
