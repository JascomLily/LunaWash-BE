using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.BLL.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByBranchAsync(string branchId)
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.BranchId == branchId && !u.IsDeleted && (u.Role.RoleName == "Staff" || u.Role.RoleName == "TechnicalStaff"))
                .ToListAsync();

            return users.Select(u => new EmployeeResponseDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName,
                BranchId = u.BranchId,
                IsActive = u.IsActive
            });
        }

        public async Task<EmployeeResponseDto?> AddEmployeeAsync(EmployeeCreateDto dto)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.RoleId);
            if (role == null) return null;

            // Generate password: Staff + Random 4 digits
            var randomNum = new Random().Next(1000, 9999);
            var password = $"Staff{randomNum}";

            var user = new User
            {
                Id = "EMP-" + DateTime.UtcNow.ToString("yyMM") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper(),
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Password = password,
                RoleId = dto.RoleId,
                BranchId = dto.BranchId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new EmployeeResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                RoleName = role.RoleName,
                BranchId = user.BranchId,
                IsActive = user.IsActive
            };
        }

        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            user.IsDeleted = true;
            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckInAsync(string employeeId, string branchId)
        {
            var today = DateTime.UtcNow.Date;
            
            // Check if already checked in today
            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == employeeId && a.AttendanceDate == today);
            
            if (existing != null) return false; // Already checked in

            var attendance = new Attendance
            {
                Id = "ATT-" + DateTime.UtcNow.ToString("yyMMdd") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper(),
                UserId = employeeId,
                BranchId = branchId,
                AttendanceDate = today,
                CheckInTime = DateTime.UtcNow,
                Status = "Present"
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckOutAsync(string employeeId)
        {
            var today = DateTime.UtcNow.Date;
            var existing = await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == employeeId && a.AttendanceDate == today);
            
            if (existing == null || existing.CheckOutTime != null) return false;

            existing.CheckOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetAttendancesByBranchAndDateAsync(string branchId, string date)
        {
            if (!DateTime.TryParse(date, out var targetDate)) return new List<AttendanceResponseDto>();
            targetDate = targetDate.Date;

            var attendances = await _context.Attendances
                .Include(a => a.User)
                .ThenInclude(u => u.Role)
                .Where(a => a.BranchId == branchId && a.AttendanceDate == targetDate)
                .ToListAsync();

            return attendances.Select(a => new AttendanceResponseDto
            {
                Id = a.Id,
                UserId = a.UserId,
                FullName = a.User.FullName,
                RoleName = a.User.Role?.RoleName ?? "",
                BranchId = a.BranchId,
                AttendanceDate = a.AttendanceDate,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = a.Status
            });
        }
    }
}
