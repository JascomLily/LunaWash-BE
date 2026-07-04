using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.BLL.Services
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _context;

        public BranchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BranchResponseDto>> GetAllBranchesAsync(bool activeOnly = false)
        {
            var query = _context.Branches.Where(b => !b.IsDeleted);

            if (activeOnly)
            {
                query = query.Where(b => b.IsActive);
            }

            var branches = await query.ToListAsync();

            return branches.Select(b => new BranchResponseDto
            {
                Id = b.Id,
                BranchName = b.BranchName,
                Address = b.Address,
                PhoneNumber = b.PhoneNumber,
                IsActive = b.IsActive,
                ImageUrl = b.ImageUrl,
                Description = b.Description,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                CreatedAt = b.CreatedAt
            });
        }

        public async Task<BranchResponseDto?> GetBranchByIdAsync(string id)
        {
            var b = await _context.Branches.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (b == null) return null;

            return new BranchResponseDto
            {
                Id = b.Id,
                BranchName = b.BranchName,
                Address = b.Address,
                PhoneNumber = b.PhoneNumber,
                IsActive = b.IsActive,
                ImageUrl = b.ImageUrl,
                Description = b.Description,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                CreatedAt = b.CreatedAt
            };
        }

        public async Task<BranchResponseDto> CreateBranchAsync(BranchCreateDto dto)
        {
            var branch = new Branch
            {
                Id = "BRN-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                BranchName = dto.BranchName,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true,
                ImageUrl = dto.ImageUrl,
                Description = dto.Description,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return new BranchResponseDto
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                PhoneNumber = branch.PhoneNumber,
                IsActive = branch.IsActive,
                ImageUrl = branch.ImageUrl,
                Description = branch.Description,
                Latitude = branch.Latitude,
                Longitude = branch.Longitude,
                CreatedAt = branch.CreatedAt
            };
        }

        public async Task<BranchResponseDto?> UpdateBranchAsync(string id, BranchUpdateDto dto)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (branch == null) return null;

            branch.BranchName = dto.BranchName;
            branch.Address = dto.Address;
            branch.PhoneNumber = dto.PhoneNumber;
            branch.IsActive = dto.IsActive;
            branch.ImageUrl = dto.ImageUrl;
            branch.Description = dto.Description;
            branch.Latitude = dto.Latitude;
            branch.Longitude = dto.Longitude;
            branch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new BranchResponseDto
            {
                Id = branch.Id,
                BranchName = branch.BranchName,
                Address = branch.Address,
                PhoneNumber = branch.PhoneNumber,
                IsActive = branch.IsActive,
                ImageUrl = branch.ImageUrl,
                Description = branch.Description,
                Latitude = branch.Latitude,
                Longitude = branch.Longitude,
                CreatedAt = branch.CreatedAt
            };
        }

        public async Task<bool> DeleteBranchAsync(string id)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (branch == null) return false;

            branch.IsDeleted = true;
            branch.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BranchEquipmentDto>> GetEquipmentsByBranchAsync(string branchId)
        {
            var equipments = await _context.Equipments
                .Where(e => e.BranchId == branchId)
                .ToListAsync();

            return equipments.Select(e => new BranchEquipmentDto
            {
                Id = e.Id,
                BranchId = e.BranchId,
                Name = e.Name,
                Category = e.Category,
                Status = e.Status,
                Priority = e.Priority
            });
        }

        public async Task<IEnumerable<BranchSlotDto>> GetSlotsByBranchAsync(string branchId)
        {
            var slots = await _context.WashSlots
                .Where(s => s.BranchId == branchId && !s.IsDeleted)
                .ToListAsync();

            return slots.Select(s => new BranchSlotDto
            {
                Id = s.Id,
                BranchId = s.BranchId,
                SlotNumber = s.SlotNumber,
                Status = s.Status
            });
        }
    }
}
