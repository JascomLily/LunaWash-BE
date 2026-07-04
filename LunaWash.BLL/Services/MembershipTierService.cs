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
    public class MembershipTierService : IMembershipTierService
    {
        private readonly ApplicationDbContext _context;

        public MembershipTierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MembershipTierResponseDto>> GetAllTiersAsync()
        {
            var tiers = await _context.MembershipTiers
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.PriorityLevel)
                .ToListAsync();

            return tiers.Select(t => MapToResponseDto(t)).ToList();
        }

        public async Task<MembershipTierResponseDto?> GetTierByIdAsync(string id)
        {
            var tier = await _context.MembershipTiers
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (tier == null) return null;

            return MapToResponseDto(tier);
        }

        public async Task<MembershipTierResponseDto> CreateTierAsync(string customId, MembershipTierCreateUpdateDto dto)
        {
            var tier = new MembershipTier
            {
                Id = customId,
                TierName = dto.TierName,
                MinPoints = dto.MinPoints,
                PointsMultiplier = dto.PointsMultiplier,
                PriorityLevel = dto.PriorityLevel,
                DiscountPercent = dto.DiscountPercent,
                KeepPoints = dto.KeepPoints,
                AdvanceBookingDays = dto.AdvanceBookingDays,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.MembershipTiers.Add(tier);
            await _context.SaveChangesAsync();

            return MapToResponseDto(tier);
        }

        public async Task<bool> UpdateTierAsync(string id, MembershipTierCreateUpdateDto dto)
        {
            var tier = await _context.MembershipTiers
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (tier == null) return false;

            tier.TierName = dto.TierName;
            tier.MinPoints = dto.MinPoints;
            tier.PointsMultiplier = dto.PointsMultiplier;
            tier.PriorityLevel = dto.PriorityLevel;
            tier.DiscountPercent = dto.DiscountPercent;
            tier.KeepPoints = dto.KeepPoints;
            tier.AdvanceBookingDays = dto.AdvanceBookingDays;
            tier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTierAsync(string id)
        {
            var tier = await _context.MembershipTiers.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (tier == null) return false;

            tier.IsDeleted = true;
            tier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdjustCustomerPointsAsync(AdjustPointsDto dto)
        {
            var profile = await _context.CustomerProfiles
                .Include(cp => cp.MembershipTier)
                .FirstOrDefaultAsync(cp => cp.UserId == dto.UserId);

            if (profile == null) return false;

            // 1. Cập nhật điểm hiện tại (không để dưới 0)
            int oldPoints = profile.CurrentPoints;
            profile.CurrentPoints = Math.Max(0, profile.CurrentPoints + dto.PointsChange);
            int actualChange = profile.CurrentPoints - oldPoints;

            // 2. Nếu là cộng điểm, cập nhật cả điểm tích lũy lũy kế
            if (dto.PointsChange > 0)
            {
                profile.AccumulatedPoints += dto.PointsChange;

                // Tự động kiểm tra nâng hạng thành viên
                var allTiers = await _context.MembershipTiers
                    .Where(t => !t.IsDeleted)
                    .OrderByDescending(t => t.MinPoints)
                    .ToListAsync();

                var qualifiedTier = allTiers.FirstOrDefault(t => profile.AccumulatedPoints >= t.MinPoints);
                if (qualifiedTier != null && qualifiedTier.Id != profile.MembershipTierId)
                {
                    profile.MembershipTierId = qualifiedTier.Id;
                }
            }

            // 3. Ghi lịch sử biến động điểm
            var pointLog = new PointHistory
            {
                UserId = dto.UserId,
                Points = actualChange,
                RemainingPoints = profile.CurrentPoints,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.PointHistories.Add(pointLog);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PointHistoryResponseDto>> GetCustomerPointHistoryAsync(string userId)
        {
            var history = await _context.PointHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return history.Select(h => new PointHistoryResponseDto
            {
                Id = h.Id,
                UserId = h.UserId,
                Points = h.Points,
                RemainingPoints = h.RemainingPoints,
                Description = h.Description,
                BookingId = h.BookingId,
                CreatedAt = h.CreatedAt
            }).ToList();
        }

        private static MembershipTierResponseDto MapToResponseDto(MembershipTier t)
        {
            return new MembershipTierResponseDto
            {
                Id = t.Id,
                TierName = t.TierName,
                MinPoints = t.MinPoints,
                PointsMultiplier = t.PointsMultiplier,
                PriorityLevel = t.PriorityLevel,
                DiscountPercent = t.DiscountPercent,
                KeepPoints = t.KeepPoints,
                AdvanceBookingDays = t.AdvanceBookingDays,
                CreatedAt = t.CreatedAt
            };
        }
    }
}
