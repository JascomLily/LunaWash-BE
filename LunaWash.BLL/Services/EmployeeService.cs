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
        private readonly IEmailService _emailService;

        public EmployeeService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.StaffProfile)
                .Where(u => !u.IsDeleted && (u.Role.RoleName == "Staff" || u.Role.RoleName == "TechnicalStaff" || u.Role.RoleName == "BranchManager"))
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
                IsActive = u.IsActive,
                Salary = u.StaffProfile?.Salary ?? 0,
                LeaveDays = u.StaffProfile?.LeaveDays ?? 0
            });
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesByBranchAsync(string branchId)
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.StaffProfile)
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
                IsActive = u.IsActive,
                Salary = u.StaffProfile?.Salary ?? 0,
                LeaveDays = u.StaffProfile?.LeaveDays ?? 0
            });
        }

        public async Task<EmployeeResponseDto?> AddEmployeeAsync(EmployeeCreateDto dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null) throw new InvalidOperationException("Email này đã được sử dụng bởi một tài khoản khác.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.RoleId);
            if (role == null) return null;

            var randomNum = new Random().Next(1000, 9999);
            var password = $"Staff{randomNum}";
            var newId = "EMP-" + DateTime.UtcNow.ToString("yyMM") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();

            var user = new User
            {
                Id = newId,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Password = password,
                RoleId = dto.RoleId,
                BranchId = dto.BranchId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var profile = new StaffProfile
            {
                UserId = newId,
                Salary = dto.Salary,
                LeaveDays = dto.LeaveDays,
                UsedLeaveDays = 0
            };

            _context.Users.Add(user);
            _context.StaffProfiles.Add(profile);
            await _context.SaveChangesAsync();

            // Send email to new employee
            string subject = "Tài khoản nhân viên LunaWash của bạn";
            string body = $@"
                <h3>Chào mừng gia nhập LunaWash!</h3>
                <p>Tài khoản đăng nhập hệ thống của bạn đã được tạo thành công.</p>
                <p><strong>Email:</strong> {dto.Email}</p>
                <p><strong>Mật khẩu:</strong> {password}</p>
                <br>
                <p>Vui lòng đăng nhập bằng Email và Mật khẩu này. Nếu bạn không yêu cầu, vui lòng bỏ qua email này.</p>
                <p>Trân trọng,<br>LunaWash Admin</p>
            ";
            
            try 
            {
                await _emailService.SendEmailAsync(dto.Email, subject, body);
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi gửi mail: " + ex.Message);
            }

            return new EmployeeResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                RoleName = role.RoleName,
                BranchId = user.BranchId,
                IsActive = user.IsActive,
                Salary = profile.Salary,
                LeaveDays = profile.LeaveDays
            };
        }

        public async Task<bool> UpdateEmployeeSalaryAsync(string id, decimal newSalary)
        {
            var profile = await _context.StaffProfiles.FirstOrDefaultAsync(p => p.UserId == id);
            if (profile == null) return false;
            
            profile.Salary = newSalary;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmployeeStatusAsync(string id, bool isActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return false;

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId && u.BranchId == branchId && !u.IsDeleted);
            if (user == null) return false;

            var today = DateTime.UtcNow.Date;
            var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.UserId == employeeId && a.AttendanceDate == today);
            
            if (attendance == null)
            {
                attendance = new Attendance
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = employeeId,
                    BranchId = branchId,
                    AttendanceDate = today,
                    CheckInTime = DateTime.UtcNow,
                    Status = "Present"
                };
                _context.Attendances.Add(attendance);
            }
            else
            {
                if (attendance.CheckInTime != null) return false;
                attendance.CheckInTime = DateTime.UtcNow;
                attendance.Status = "Present";
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckOutAsync(string employeeId)
        {
            var today = DateTime.UtcNow.Date;
            var attendance = await _context.Attendances.FirstOrDefaultAsync(a => a.UserId == employeeId && a.AttendanceDate == today);
            
            if (attendance == null || attendance.CheckInTime == null || attendance.CheckOutTime != null) return false;

            attendance.CheckOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetAttendancesByBranchAndDateAsync(string branchId, string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate)) return new List<AttendanceResponseDto>();
            var targetDate = parsedDate.Date;

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
                RoleName = a.User.Role.RoleName,
                BranchId = a.BranchId,
                AttendanceDate = a.AttendanceDate,
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Status = a.Status
            });
        }
    }
}
