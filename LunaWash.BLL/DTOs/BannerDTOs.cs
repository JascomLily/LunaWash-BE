using System;
using System.Collections.Generic;

namespace LunaWash.BLL.DTOs;

public class BannerDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? VoucherId { get; set; }
    public VoucherDto? Voucher { get; set; }
}

public class SaveBannerDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? VoucherId { get; set; }
}