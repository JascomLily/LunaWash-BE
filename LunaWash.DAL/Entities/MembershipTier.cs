using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class MembershipTier
{
    public string Id { get; set; } = null!;

    public string TierName { get; set; } = null!;

    public int MinPoints { get; set; }

    public decimal PointsMultiplier { get; set; }

    public int PriorityLevel { get; set; }

    public decimal DiscountPercent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CustomerProfile> CustomerProfiles { get; set; } = new List<CustomerProfile>();
}
