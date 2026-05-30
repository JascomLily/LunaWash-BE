using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class ServicePrice
{
    public string Id { get; set; } = null!;

    public string ServiceId { get; set; } = null!;

    public string VehicleTypeId { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public int PointsRewarded { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual WashService Service { get; set; } = null!;

    public virtual VehicleType VehicleType { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
