using System;

namespace LunaWash.DAL.Entities;

public partial class BookingService
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string BookingId { get; set; } = null!;

    public string WashServiceId { get; set; } = null!;

    public decimal PriceAtTime { get; set; }

    public int DurationAtTime { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual WashService WashService { get; set; } = null!;
}
