using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;

namespace LunaWash.BLL.Services
{
    public class BannerService : IBannerService
    {
        private readonly ApplicationDbContext _context;

        public BannerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BannerResponseDto>> GetAllBannersAsync(bool activeOnly = false)
        {
            var query = _context.Banners.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(b => b.IsActive);
            }

            var banners = await query.OrderBy(b => b.Position).ToListAsync();

            return banners.Select(b => MapToResponseDto(b)).ToList();
        }

        public async Task<BannerResponseDto?> GetBannerByIdAsync(string id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(b => b.Id == id);
            if (banner == null) return null;

            return MapToResponseDto(banner);
        }

        public async Task<BannerResponseDto> CreateBannerAsync(BannerCreateUpdateDto dto)
        {
            var banner = new Banner
            {
                Id = "BNR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                ImageUrl = dto.Url,
                Title = dto.Url.Length > 30 ? dto.Url.Substring(0, 30) : dto.Url,
                RedirectUrl = dto.PromoCode,
                Position = dto.Position,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return MapToResponseDto(banner);
        }

        public async Task<bool> UpdateBannerAsync(string id, BannerCreateUpdateDto dto)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(b => b.Id == id);
            if (banner == null) return false;

            banner.ImageUrl = dto.Url;
            banner.RedirectUrl = dto.PromoCode;
            banner.Position = dto.Position;
            banner.IsActive = dto.IsActive;
            banner.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBannerAsync(string id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(b => b.Id == id);
            if (banner == null) return false;

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveBannersBulkAsync(IEnumerable<BannerCreateUpdateDto> dtos)
        {
            // Truncate and replace or update positions
            // Let's clear existing banners and replace with the new list to keep it simple and consistent with FE localStorage behavior.
            var existing = await _context.Banners.ToListAsync();
            _context.Banners.RemoveRange(existing);

            int index = 1;
            foreach (var dto in dtos)
            {
                var banner = new Banner
                {
                    Id = "BNR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    ImageUrl = dto.Url,
                    Title = "Banner " + index,
                    RedirectUrl = dto.PromoCode,
                    Position = dto.Position > 0 ? dto.Position : index,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Banners.Add(banner);
                index++;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static BannerResponseDto MapToResponseDto(Banner b)
        {
            return new BannerResponseDto
            {
                Id = b.Id,
                Url = b.ImageUrl,
                PromoCode = b.RedirectUrl,
                Position = b.Position,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt
            };
        }
    }
}
