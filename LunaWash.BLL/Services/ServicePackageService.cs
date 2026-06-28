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
    public class ServicePackageService : IServicePackageService
    {
        private readonly ApplicationDbContext _context;

        public ServicePackageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServicePackageResponseDto>> GetAllPackagesAsync(bool activeOnly = false)
        {
            var query = _context.ServicePackages
                .Include(p => p.PackageServices)
                .ThenInclude(ps => ps.WashService)
                .Where(p => !p.IsDeleted);

            if (activeOnly)
            {
                query = query.Where(p => p.IsActive);
            }

            var packages = await query.ToListAsync();

            return packages.Select(p => MapToResponseDto(p)).ToList();
        }

        public async Task<ServicePackageResponseDto?> GetPackageByIdAsync(string id)
        {
            var package = await _context.ServicePackages
                .Include(p => p.PackageServices)
                .ThenInclude(ps => ps.WashService)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (package == null) return null;

            return MapToResponseDto(package);
        }

        public async Task<ServicePackageResponseDto> CreatePackageAsync(ServicePackageCreateUpdateDto dto)
        {
            var package = new ServicePackage
            {
                Id = "PKG-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ServicePackages.Add(package);

            if (dto.ServiceIds != null && dto.ServiceIds.Any())
            {
                foreach (var serviceId in dto.ServiceIds)
                {
                    _context.PackageServices.Add(new PackageService
                    {
                        PackageId = package.Id,
                        ServiceId = serviceId
                    });
                }
            }

            await _context.SaveChangesAsync();

            // Reload package with navigation properties
            var savedPackage = await _context.ServicePackages
                .Include(p => p.PackageServices)
                .ThenInclude(ps => ps.WashService)
                .FirstAsync(p => p.Id == package.Id);

            return MapToResponseDto(savedPackage);
        }

        public async Task<bool> UpdatePackageAsync(string id, ServicePackageCreateUpdateDto dto)
        {
            var package = await _context.ServicePackages
                .Include(p => p.PackageServices)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (package == null) return false;

            package.Name = dto.Name;
            package.Description = dto.Description;
            package.Price = dto.Price;
            package.IsActive = dto.IsActive;
            package.UpdatedAt = DateTime.UtcNow;

            // Remove old services
            _context.PackageServices.RemoveRange(package.PackageServices);

            // Add new services
            if (dto.ServiceIds != null && dto.ServiceIds.Any())
            {
                foreach (var serviceId in dto.ServiceIds)
                {
                    _context.PackageServices.Add(new PackageService
                    {
                        PackageId = package.Id,
                        ServiceId = serviceId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePackageAsync(string id)
        {
            var package = await _context.ServicePackages.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (package == null) return false;

            package.IsDeleted = true;
            package.IsActive = false;
            package.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private static ServicePackageResponseDto MapToResponseDto(ServicePackage p)
        {
            return new ServicePackageResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                ServiceIds = p.PackageServices.Select(ps => ps.ServiceId).ToList(),
                ServiceNames = p.PackageServices.Select(ps => ps.WashService?.ServiceName ?? "Dịch vụ").ToList()
            };
        }
    }
}
