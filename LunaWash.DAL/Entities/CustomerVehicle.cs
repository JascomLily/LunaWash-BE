using System;

namespace LunaWash.DAL.Entities;

public partial class CustomerVehicle
{
    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public string? VehicleTypeId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string VehicleModel { get; set; } = null!;

    public string? Color { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual VehicleType? VehicleType { get; set; }
}
