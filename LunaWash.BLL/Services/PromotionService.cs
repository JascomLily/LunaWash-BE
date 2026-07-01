using LunaWash.BLL.DTOs;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunaWash.BLL.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly ApplicationDbContext _context;

        public PromotionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PromotionResponseDTO> CreatePromotionAsync(CreatePromotionDTO dto)
        {
            if (await _context.Promotions.AnyAsync(p => p.Code.ToUpper() == dto.Code.ToUpper() && p.IsActive))
            {
                throw new Exception("Mã khuyến mãi này đang được sử dụng.");
            }

            var promotion = new Promotion
            {
                Name = dto.Name,
                Code = dto.Code.ToUpper(),
                DiscountPercent = dto.DiscountPercent,
                MaxUsage = dto.MaxUsage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true
            };

            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            return MapToResponse(promotion);
        }

        public async Task<List<PromotionResponseDTO>> GetAllPromotionsAsync()
        {
            var promotions = await _context.Promotions
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return promotions.Select(MapToResponse).ToList();
        }

        public async Task<ValidatePromotionDTO> ValidatePromoCodeAsync(string code)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code.ToUpper() == code.ToUpper() && p.IsActive && !p.IsDeleted);

            if (promotion == null)
                throw new Exception("Mã khuyến mãi không tồn tại hoặc đã bị vô hiệu hóa.");

            if (DateTime.UtcNow < promotion.StartDate)
                throw new Exception("Mã khuyến mãi chưa bắt đầu.");

            if (DateTime.UtcNow > promotion.EndDate)
                throw new Exception("Mã khuyến mãi đã hết hạn.");

            if (promotion.MaxUsage.HasValue && promotion.CurrentUsage >= promotion.MaxUsage.Value)
                throw new Exception("Mã khuyến mãi đã hết lượt sử dụng.");

            return new ValidatePromotionDTO
            {
                Code = promotion.Code,
                Name = promotion.Name,
                DiscountPercent = promotion.DiscountPercent
            };
        }

        private PromotionResponseDTO MapToResponse(Promotion p)
        {
            string status = "Đang chạy";
            if (!p.IsActive) status = "Đã dừng";
            else if (DateTime.UtcNow < p.StartDate) status = "Sắp diễn ra";
            else if (DateTime.UtcNow > p.EndDate) status = "Đã kết thúc";
            else if (p.MaxUsage.HasValue && p.CurrentUsage >= p.MaxUsage.Value) status = "Hết lượt";

            return new PromotionResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DiscountPercent = p.DiscountPercent,
                MaxUsage = p.MaxUsage,
                CurrentUsage = p.CurrentUsage,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive,
                Status = status
            };
        }
    }
}
