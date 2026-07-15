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
    public async Task<IActionResult> GetBanners([FromQuery] string platform = "Web")
    {
        var banners = await _context.Banners
            .Include(b => b.Voucher)
            .Where(b => b.PlatformType == platform)
            .ToListAsync();

        var dtos = banners.Select(b => new BannerDto
        {
            Id = b.Id,
            ImageUrl = b.ImageUrl,
            VoucherId = b.VoucherId,
            PlatformType = b.PlatformType,
            IsHidden = b.IsHidden,
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
        // Frontend gửi toàn bộ danh sách banner của cả Web và App trong một request
        // Vì vậy, chúng ta sẽ xóa toàn bộ banner hiện tại và lưu lại danh sách mới
        var existingBanners = await _context.Banners.ToListAsync();
        _context.Banners.RemoveRange(existingBanners);

        var newBanners = bannerDtos.Select(dto => new Banner
        {
            ImageUrl = dto.ImageUrl,
            VoucherId = string.IsNullOrEmpty(dto.VoucherId) ? null : dto.VoucherId,
            PlatformType = dto.PlatformType,
            IsHidden = dto.IsHidden || string.IsNullOrEmpty(dto.ImageUrl)
        }).ToList();

        await _context.Banners.AddRangeAsync(newBanners);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Banners saved successfully" });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadBannerPhoto([FromForm] Microsoft.AspNetCore.Http.IFormFile file, [FromServices] LunaWash.BLL.Interfaces.IPhotoService photoService)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        var url = await photoService.UploadPhotoAsync(file);
        if (string.IsNullOrEmpty(url)) return BadRequest("Failed to upload image");

        return Ok(new { success = true, url });
    }
}