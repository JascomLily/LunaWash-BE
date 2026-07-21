using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IStaffManagementService
    {
        Task<IEnumerable<StaffScheduleDto>> GetSchedulesByBranchAsync(string branchId);
        Task<bool> SaveSchedulesAsync(string branchId, string managerId, List<StaffScheduleDto> templates);
        Task<IEnumerable<ScheduleHistoryLogDto>> GetHistoryByBranchAsync(string branchId);
        Task<bool> SaveAttendanceAsync(string branchId, string shift, List<AttendanceEntryDto> attendances);
    }
}
