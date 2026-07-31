using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync();
        Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByBranchAsync(string branchId);
        Task<EmployeeResponseDto?> AddEmployeeAsync(EmployeeCreateDto dto);
        Task<bool> DeleteEmployeeAsync(string id);
        
        // Admin Features
        Task<bool> UpdateEmployeeSalaryAsync(string id, decimal newSalary);
        Task<bool> UpdateEmployeeStatusAsync(string id, bool isActive);

        Task<bool> CheckInAsync(string employeeId, string branchId);
        Task<bool> CheckOutAsync(string employeeId);
        Task<IEnumerable<AttendanceResponseDto>> GetAttendancesByBranchAndDateAsync(string branchId, string date);
        Task<IEnumerable<AttendanceResponseDto>> GetWeeklyLeavesByBranchAsync(string branchId, string date);
    }
}
