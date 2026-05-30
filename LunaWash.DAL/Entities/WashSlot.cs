using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class WashSlot
{
    public string Id { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string SlotNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Branch Branch { get; set; } = null!;
}
