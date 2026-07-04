using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceTaskDetailDto>> GetTasksByTechnicianAsync(string techId);
        Task<IEnumerable<MaintenanceTaskDetailDto>> GetTasksByBranchAsync(string branchId);
        Task<bool> UpdateTaskStatusAsync(string id, string techId, MaintenanceTaskUpdateStatusDto dto);
        Task<bool> ConfirmTaskCompletionAsync(string id);
        Task<bool> AssignTaskAsync(string id, string assignedToId, string priority);
        Task<EquipmentCheckLogResponseDto?> CreateCheckLogAsync(string branchId, string techId, EquipmentCheckLogCreateDto dto);
        Task<IEnumerable<EquipmentCheckLogResponseDto>> GetCheckLogsByBranchAsync(string branchId);
    }
}
