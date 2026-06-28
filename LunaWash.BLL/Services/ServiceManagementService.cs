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
                .Include(s => s.ServiceFeatures)
                .Where(s => !s.IsDeleted)
                .ToListAsync();

            return services.Select(s => new WashServiceDto
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                Description = s.Description,
                ServiceType = s.ServiceType,
                IconName = s.IconName,
                IsPopular = s.IsPopular,
                IsActive = s.IsActive,
                ServiceFeatures = s.ServiceFeatures.OrderBy(f => f.DisplayOrder).Select(f => f.FeatureText).ToList(),
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
                .Include(s => s.ServiceFeatures)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (service == null) return null;

            return new WashServiceDto
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                ServiceType = service.ServiceType,
                IconName = service.IconName,
                IsPopular = service.IsPopular,
                IsActive = service.IsActive,
                ServiceFeatures = service.ServiceFeatures.OrderBy(f => f.DisplayOrder).Select(f => f.FeatureText).ToList(),
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
            if (dto.ServiceFeatures.Count > 5)
            {
                throw new ArgumentException("A service can only have a maximum of 5 features.");
            }

            var service = new WashService
            {
                Id = "SRV-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                ServiceName = dto.ServiceName,
                Description = dto.Description,
                ServiceType = dto.ServiceType,
                IconName = dto.IconName,
                IsPopular = dto.IsPopular,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            for (int i = 0; i < dto.ServiceFeatures.Count; i++)
            {
                service.ServiceFeatures.Add(new ServiceFeature
                {
                    Id = Guid.NewGuid().ToString(),
                    FeatureText = dto.ServiceFeatures[i],
                    DisplayOrder = i
                });
            }

            _context.WashServices.Add(service);
            await _context.SaveChangesAsync();

            return new WashServiceDto
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                ServiceType = service.ServiceType,
                IconName = service.IconName,
                IsPopular = service.IsPopular,
                IsActive = service.IsActive,
                ServiceFeatures = service.ServiceFeatures.OrderBy(f => f.DisplayOrder).Select(f => f.FeatureText).ToList()
            };
        }

        public async Task<bool> UpdateServiceAsync(string id, WashServiceUpdateDto dto)
        {
            var service = await _context.WashServices.Include(s => s.ServiceFeatures).FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
            if (service == null) return false;

            if (dto.ServiceFeatures.Count > 5)
            {
                throw new ArgumentException("A service can only have a maximum of 5 features.");
            }

            service.ServiceName = dto.ServiceName;
            service.Description = dto.Description;
            service.ServiceType = dto.ServiceType;
            service.IconName = dto.IconName;
            service.IsPopular = dto.IsPopular;
            service.IsActive = dto.IsActive;
            service.UpdatedAt = DateTime.UtcNow;

            _context.ServiceFeatures.RemoveRange(service.ServiceFeatures);
            service.ServiceFeatures.Clear();

            for (int i = 0; i < dto.ServiceFeatures.Count; i++)
            {
                service.ServiceFeatures.Add(new ServiceFeature
                {
                    Id = Guid.NewGuid().ToString(),
                    FeatureText = dto.ServiceFeatures[i],
                    DisplayOrder = i
                });
            }

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
