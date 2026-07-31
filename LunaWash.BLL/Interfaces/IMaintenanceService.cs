using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IMaintenanceService
    {
        Task<MaintenanceResponse> CreateMaintenanceTaskAsync(CreateMaintenanceRequest request);
        Task<IEnumerable<MaintenanceResponse>> GetMaintenanceTasksByBranchAsync(string branchId);
        Task<IEnumerable<MaintenanceResponse>> GetMaintenanceTasksByAssigneeAsync(string assigneeId);
        Task<MaintenanceResponse?> GetMaintenanceTaskByIdAsync(string taskId);
        Task<bool> UpdateMaintenanceTaskStatusAsync(string taskId, UpdateMaintenanceStatusRequest request);
        Task<bool> AssignMaintenanceTaskAsync(string taskId, string assigneeId);
    }
}
