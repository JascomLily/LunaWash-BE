using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using LunaWash.BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LunaWash.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BannersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BannersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetBanners()
    {
        var banners = await _context.Banners
            .Include(b => b.Voucher)
            .ToListAsync();

        var dtos = banners.Select(b => new BannerDto
        {
            Id = b.Id,
            ImageUrl = b.ImageUrl,
            VoucherId = b.VoucherId,
            Voucher = b.Voucher != null ? new VoucherDto
            {
                Id = b.Voucher.Id,
                VoucherName = b.Voucher.VoucherName,
                Description = b.Voucher.Description,
                DiscountValue = b.Voucher.DiscountValue,
                ExpiryDate = b.Voucher.ExpiryDate
            } : null
        });

        return Ok(new { success = true, data = dtos });
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveBanners([FromBody] List<SaveBannerDto> bannerDtos)
    {
        var existingBanners = await _context.Banners.ToListAsync();
        _context.Banners.RemoveRange(existingBanners);

        var newBanners = bannerDtos.Select(dto => new Banner
        {
            ImageUrl = dto.ImageUrl,
            VoucherId = string.IsNullOrEmpty(dto.VoucherId) ? null : dto.VoucherId
        }).ToList();

        await _context.Banners.AddRangeAsync(newBanners);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Banners saved successfully" });
    }
}