using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleResponseDTO>> GetUserVehiclesAsync(string userId);
        Task<VehicleResponseDTO?> AddVehicleAsync(string userId, CreateVehicleRequestDTO dto);
        Task<bool> DeleteVehicleAsync(string userId, string vehicleId);
    }
}
