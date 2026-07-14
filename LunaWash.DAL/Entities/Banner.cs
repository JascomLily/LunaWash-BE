using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LunaWash.DAL.Entities;

public class Banner
{
    [Key]
    public int Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? VoucherId { get; set; }

    [ForeignKey("VoucherId")]
    public virtual Voucher? Voucher { get; set; }
}