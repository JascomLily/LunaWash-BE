using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class ServicePackage
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<PackageService> PackageServices { get; set; } = new List<PackageService>();
}
