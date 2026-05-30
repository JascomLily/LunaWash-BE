using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class VehicleType
{
    public string Id { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();
}
