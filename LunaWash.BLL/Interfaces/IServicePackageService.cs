using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IServicePackageService
    {
        Task<IEnumerable<ServicePackageResponseDto>> GetAllPackagesAsync(bool activeOnly = false);
        Task<ServicePackageResponseDto?> GetPackageByIdAsync(string id);
        Task<ServicePackageResponseDto> CreatePackageAsync(ServicePackageCreateUpdateDto dto);
        Task<bool> UpdatePackageAsync(string id, ServicePackageCreateUpdateDto dto);
        Task<bool> DeletePackageAsync(string id);
    }
}
