using System;
using System.Collections.Generic;

namespace LunaWash.DAL.Entities;

public partial class Voucher
{
    public string Id { get; set; } = null!;

    public string VoucherName { get; set; } = null!;

    public string? Description { get; set; }

    public int PointsRequired { get; set; }

    public decimal DiscountValue { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CustomerVoucher> CustomerVouchers { get; set; } = new List<CustomerVoucher>();
}
