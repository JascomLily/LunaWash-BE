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
    public class ServiceManagementService : IServiceManagementService
    {
        private readonly ApplicationDbContext _context;

        public ServiceManagementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WashServiceDto>> GetAllServicesAsync()
        {
            var services = await _context.WashServices
                .Include(s => s.ServicePrices)
                .Where(s => !s.IsDeleted)
                .ToListAsync();

            return services.Select(s => new WashServiceDto
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                Description = s.Description,
                IsActive = s.IsActive,
                Prices = s.ServicePrices.Where(p => !p.IsDeleted).Select(p => new ServicePriceDto
                {
                    Id = p.Id,
                    VehicleTypeId = p.VehicleTypeId,
                    Price = p.Price,
                    DurationMinutes = p.DurationMinutes,
                    PointsRewarded = p.PointsRewarded
                }).ToList()
            });
        }

        public async Task<WashServiceDto?> GetServiceByIdAsync(string id)
        {
            var service = await _context.WashServices
                .Include(s => s.ServicePrices)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (service == null) return null;

            return new WashServiceDto
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                IsActive = service.IsActive,
                Prices = service.ServicePrices.Where(p => !p.IsDeleted).Select(p => new ServicePriceDto
                {
                    Id = p.Id,
                    VehicleTypeId = p.VehicleTypeId,
                    Price = p.Price,
                    DurationMinutes = p.DurationMinutes,
                    PointsRewarded = p.PointsRewarded
                }).ToList()
            };
        }

        public async Task<WashServiceDto> CreateServiceAsync(WashServiceCreateDto dto)
        {
            var service = new WashService
            {
                Id = "SRV-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                ServiceName = dto.ServiceName,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.WashServices.Add(service);
            await _context.SaveChangesAsync();

            return new WashServiceDto
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                IsActive = service.IsActive
            };
        }

        public async Task<bool> UpdateServiceAsync(string id, WashServiceUpdateDto dto)
        {
            var service = await _context.WashServices.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (service == null) return false;

            service.ServiceName = dto.ServiceName;
            service.Description = dto.Description;
            service.IsActive = dto.IsActive;
            service.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceAsync(string id)
        {
            var service = await _context.WashServices.FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (service == null) return false;

            service.IsDeleted = true;
            service.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddOrUpdateServicePriceAsync(string serviceId, ServicePriceCreateUpdateDto dto)
        {
            var service = await _context.WashServices.FirstOrDefaultAsync(s => s.Id == serviceId && !s.IsDeleted);
            if (service == null) return false;

            var existingPrice = await _context.ServicePrices
                .FirstOrDefaultAsync(p => p.ServiceId == serviceId && p.VehicleTypeId == dto.VehicleTypeId && !p.IsDeleted);

            if (existingPrice != null)
            {
                existingPrice.Price = dto.Price;
                existingPrice.DurationMinutes = dto.DurationMinutes;
                existingPrice.PointsRewarded = dto.PointsRewarded;
                existingPrice.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var newPrice = new ServicePrice
                {
                    Id = "PRC-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    ServiceId = serviceId,
                    VehicleTypeId = dto.VehicleTypeId,
                    Price = dto.Price,
                    DurationMinutes = dto.DurationMinutes,
                    PointsRewarded = dto.PointsRewarded,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                _context.ServicePrices.Add(newPrice);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
