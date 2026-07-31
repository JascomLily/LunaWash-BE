using System;

namespace LunaWash.BLL.DTOs;

public class VoucherDto
{
    public string Id { get; set; } = string.Empty;
    public string VoucherName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PointsRequired { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
}

public class CustomerVoucherDto
{
    public string Id { get; set; } = string.Empty;
    public string VoucherId { get; set; } = string.Empty;
    public VoucherDto Voucher { get; set; } = null!;
    public DateTime RedeemedDate { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
}

public class CreateVoucherDto
{
    public string? Id { get; set; }
    public string VoucherName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PointsRequired { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime ExpiryDate { get; set; }
}
