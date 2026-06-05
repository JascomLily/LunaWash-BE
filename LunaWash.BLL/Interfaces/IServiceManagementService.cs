using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IServiceManagementService
    {
        Task<IEnumerable<WashServiceDto>> GetAllServicesAsync();
        Task<WashServiceDto?> GetServiceByIdAsync(string id);
        Task<WashServiceDto> CreateServiceAsync(WashServiceCreateDto dto);
        Task<bool> UpdateServiceAsync(string id, WashServiceUpdateDto dto);
        Task<bool> DeleteServiceAsync(string id);
        Task<bool> AddOrUpdateServicePriceAsync(string serviceId, ServicePriceCreateUpdateDto dto);
    }
}
