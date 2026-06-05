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
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VehicleResponseDTO>> GetUserVehiclesAsync(string userId)
        {
            return await _context.CustomerVehicles
                .Include(v => v.VehicleType)
                .Where(v => v.CustomerId == userId && !v.IsDeleted)
                .Select(v => new VehicleResponseDTO
                {
                    Id = v.Id,
                    Name = v.VehicleModel,
                    License = v.LicensePlate,
                    Color = v.Color,
                    VehicleTypeId = v.VehicleTypeId,
                    VehicleTypeName = v.VehicleType != null ? v.VehicleType.TypeName : null
                })
                .ToListAsync();
        }

        public async Task<VehicleResponseDTO?> AddVehicleAsync(string userId, CreateVehicleRequestDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.IsDeleted) return null;

            var existingLicense = await _context.CustomerVehicles
                .FirstOrDefaultAsync(v => v.LicensePlate == dto.License);
            
            if (existingLicense != null)
            {
                throw new InvalidOperationException("Biển số xe này đã được đăng ký trong hệ thống.");
            }

            var newVehicle = new CustomerVehicle
            {
                Id = "VEH-CUST-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                CustomerId = userId,
                VehicleModel = dto.Name,
                LicensePlate = dto.License,
                Color = dto.Color,
                VehicleTypeId = dto.VehicleTypeId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.CustomerVehicles.Add(newVehicle);
            await _context.SaveChangesAsync();

            string? typeName = null;
            if (!string.IsNullOrEmpty(newVehicle.VehicleTypeId))
            {
                var vType = await _context.VehicleTypes.FindAsync(newVehicle.VehicleTypeId);
                typeName = vType?.TypeName;
            }

            return new VehicleResponseDTO
            {
                Id = newVehicle.Id,
                Name = newVehicle.VehicleModel,
                License = newVehicle.LicensePlate,
                Color = newVehicle.Color,
                VehicleTypeId = newVehicle.VehicleTypeId,
                VehicleTypeName = typeName
            };
        }

        public async Task<bool> DeleteVehicleAsync(string userId, string vehicleId)
        {
            var vehicle = await _context.CustomerVehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == userId);

            if (vehicle == null) return false;

            _context.CustomerVehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
