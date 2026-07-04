using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IStaffManagementService
    {
        Task<IEnumerable<UserBranchResponseDto>> GetEmployeesByBranchAsync(string branchId);
        Task<IEnumerable<DailyAttendanceResponseDto>> GetAttendanceAsync(string branchId, DateTime date, string shift);
        Task<bool> SaveAttendanceAsync(SaveAttendanceDto dto);
        Task<IEnumerable<ShiftTemplateResponseDto>> GetShiftTemplatesAsync(string branchId);
        Task<bool> SaveShiftTemplatesAsync(string branchId, string managerId, SaveShiftTemplatesDto dto);
        Task<IEnumerable<ScheduleHistoryResponseDto>> GetScheduleHistoryAsync(string branchId);
    }
}
