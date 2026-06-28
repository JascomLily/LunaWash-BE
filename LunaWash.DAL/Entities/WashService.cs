using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class WashService
{
    public string Id { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public string ServiceType { get; set; } = "Package"; // "Package" or "AddOn"

    public string? IconName { get; set; }

    public bool IsPopular { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();

    public virtual ICollection<ServiceFeature> ServiceFeatures { get; set; } = new List<ServiceFeature>();

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}
