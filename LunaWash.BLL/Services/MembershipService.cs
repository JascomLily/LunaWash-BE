using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.BLL.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly ApplicationDbContext _context;

        public MembershipService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MembershipTierDto>> GetAllTiersAsync()
        {
            var tiers = await _context.MembershipTiers
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.MinPoints)
                .ToListAsync();

            var customerCounts = await _context.CustomerProfiles
                .GroupBy(c => c.MembershipTierId)
                .Select(g => new { TierId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.TierId, g => g.Count);

            return tiers.Select(t => new MembershipTierDto
            {
                Id = t.Id,
                TierName = t.TierName,
                MinPoints = t.MinPoints,
                PointsMultiplier = t.PointsMultiplier,
                PriorityLevel = t.PriorityLevel,
                DiscountPercent = t.DiscountPercent,
                MaxBookingDays = t.MaxBookingDays,
                CustomerCount = customerCounts.ContainsKey(t.Id) ? customerCounts[t.Id] : 0
            });
        }

        public async Task<bool> UpdateTierAsync(string id, MembershipTierUpdateDto dto)
        {
            var tier = await _context.MembershipTiers.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (tier == null) return false;

            tier.MinPoints = dto.MinPoints;
            tier.PointsMultiplier = dto.PointsMultiplier;
            tier.PriorityLevel = dto.PriorityLevel;
            tier.DiscountPercent = dto.DiscountPercent;
            tier.MaxBookingDays = dto.MaxBookingDays;
            tier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SyncCustomerTiersAsync()
        {
            var tiers = await _context.MembershipTiers
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.MinPoints) // Highest points first
                .ToListAsync();

            if (!tiers.Any()) return false;

            var customers = await _context.CustomerProfiles.ToListAsync();
            
            foreach (var customer in customers)
            {
                // Find the appropriate tier for this customer based on AccumulatedPoints
                var appropriateTier = tiers.FirstOrDefault(t => customer.AccumulatedPoints >= t.MinPoints) 
                                   ?? tiers.Last(); // Fallback to lowest tier if somehow they have less than 0

                if (customer.MembershipTierId != appropriateTier.Id)
                {
                    customer.MembershipTierId = appropriateTier.Id;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
