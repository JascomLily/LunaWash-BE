using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IEquipmentService
    {
        Task<EquipmentDashboardDTO> GetDashboardAsync(string branchId, string? washSlotId = null);
        Task<bool> UpdateEquipmentStatusAsync(string equipmentId, string branchId, string status);
        Task<bool> UpdateEquipmentScheduleAsync(string equipmentId, string branchId, string nextMaintenance);
        Task<bool> UpdateEquipmentPriorityAsync(string equipmentId, string branchId, string priority);
        Task<bool> CreateIncidentTicketAsync(string branchId, CreateIncidentDTO dto);
        Task<bool> ToggleTaskStatusAsync(string taskId, string branchId);
        Task<bool> UpdateTaskStatusAsync(string taskId, string branchId, string status);
        Task<bool> CreateReportAsync(string branchId, string equipmentId, string issueName, string description, string status, string taskStatus);
    }
}
