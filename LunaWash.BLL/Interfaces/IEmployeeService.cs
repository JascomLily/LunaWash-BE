using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByBranchAsync(string branchId);
        Task<EmployeeResponseDto?> AddEmployeeAsync(EmployeeCreateDto dto);
        Task<bool> DeleteEmployeeAsync(string id);
        
        Task<bool> CheckInAsync(string employeeId, string branchId);
        Task<bool> CheckOutAsync(string employeeId);
        Task<IEnumerable<AttendanceResponseDto>> GetAttendancesByBranchAndDateAsync(string branchId, string date);
    }
}
