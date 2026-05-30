using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class WashService
{
    public string Id { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();
}
