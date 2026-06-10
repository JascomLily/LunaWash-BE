using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class Branch
{
    public string Id { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<WashSlot> WashSlots { get; set; } = new List<WashSlot>();
}
