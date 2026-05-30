using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class CustomerProfile
{
    public string UserId { get; set; } = null!;

    public int CurrentPoints { get; set; }

    public int AccumulatedPoints { get; set; }

    public string MembershipTierId { get; set; } = null!;

    public virtual MembershipTier MembershipTier { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
