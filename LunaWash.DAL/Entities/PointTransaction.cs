using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class PointTransaction
{
    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public int Points { get; set; }

    public string TransactionType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User Customer { get; set; } = null!;
}
