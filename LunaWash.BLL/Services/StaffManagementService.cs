using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;

namespace LunaWash.BLL.Services
{
    public class StaffManagementService : IStaffManagementService
    {
        private readonly ApplicationDbContext _context;

        public StaffManagementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserBranchResponseDto>> GetEmployeesByBranchAsync(string branchId)
        {
            var employees = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.BranchId == branchId && !u.IsDeleted && u.Role.RoleName != "Customer")
                .ToListAsync();

            return employees.Select(emp => new UserBranchResponseDto
            {
                Id = emp.Id,
                FullName = emp.FullName,
                Email = emp.Email,
                PhoneNumber = emp.PhoneNumber,
                RoleName = emp.Role.RoleName,
                Salary = emp.Salary ?? 6000000, 
                LeaveDays = emp.LeaveDays ?? 0,
                IsActive = emp.IsActive
            }).ToList();
        }

        public async Task<IEnumerable<DailyAttendanceResponseDto>> GetAttendanceAsync(string branchId, DateTime date, string shift)
        {
            var employees = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.BranchId == branchId && !u.IsDeleted && u.Role.RoleName != "Customer" && u.Role.RoleName != "BranchManager")
                .ToListAsync();

            var employeeIds = employees.Select(e => e.Id).ToList();

            var attendanceRecords = await _context.DailyAttendances
                .Where(a => employeeIds.Contains(a.EmployeeId) && a.Date.Date == date.Date && a.Shift == shift)
                .ToListAsync();

            var result = new List<DailyAttendanceResponseDto>();
            foreach (var emp in employees)
            {
                var record = attendanceRecords.FirstOrDefault(r => r.EmployeeId == emp.Id);
                result.Add(new DailyAttendanceResponseDto
                {
                    EmployeeId = emp.Id,
                    FullName = emp.FullName,
                    RoleName = emp.Role.RoleName,
                    Status = record?.Status ?? "Vắng mặt",
                    CheckInTime = record?.CheckInTime?.AddHours(7).ToString("HH:mm"),
                    Notes = record?.Notes
                });
            }
            return result;
        }

        public async Task<bool> SaveAttendanceAsync(SaveAttendanceDto dto)
        {
            var today = DateTime.UtcNow.Date;
            var employeeIds = dto.Attendances.Select(a => a.EmployeeId).ToList();
            var existingRecords = await _context.DailyAttendances
                .Where(a => employeeIds.Contains(a.EmployeeId) && a.Date.Date == today && a.Shift == dto.Shift)
                .ToListAsync();

            foreach (var att in dto.Attendances)
            {
                var record = existingRecords.FirstOrDefault(r => r.EmployeeId == att.EmployeeId);

                if (record != null)
                {
                    record.Status = att.Status;
                    record.Notes = att.Note;
                    if (att.Status == "Có mặt" && record.CheckInTime == null)
                    {
                        record.CheckInTime = DateTime.UtcNow;
                    }
                }
                else
                {
                    record = new DailyAttendance
                    {
                        Id = "ATT-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        EmployeeId = att.EmployeeId,
                        Date = today,
                        Shift = dto.Shift,
                        Status = att.Status,
                        Notes = att.Note,
                        CheckInTime = att.Status == "Có mặt" ? DateTime.UtcNow : null
                    };
                    _context.DailyAttendances.Add(record);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ShiftTemplateResponseDto>> GetShiftTemplatesAsync(string branchId)
        {
            var employees = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.BranchId == branchId && !u.IsDeleted && u.Role.RoleName != "Customer" && u.Role.RoleName != "BranchManager")
                .ToListAsync();

            var employeeIds = employees.Select(e => e.Id).ToList();

            var templates = await _context.EmployeeScheduleTemplates
                .Where(t => employeeIds.Contains(t.EmployeeId))
                .ToListAsync();

            var result = new List<ShiftTemplateResponseDto>();
            foreach (var emp in employees)
            {
                var temp = templates.FirstOrDefault(t => t.EmployeeId == emp.Id);
                result.Add(new ShiftTemplateResponseDto
                {
                    Id = emp.Id,
                    Shift = temp?.Shift ?? "Ca sáng",
                    DayOff = temp?.DayOff ?? "Thứ Hai"
                });
            }
            return result;
        }

        public async Task<bool> SaveShiftTemplatesAsync(string branchId, string managerId, SaveShiftTemplatesDto dto)
        {
            foreach (var item in dto.Templates)
            {
                var existing = await _context.EmployeeScheduleTemplates
                    .FirstOrDefaultAsync(t => t.EmployeeId == item.EmployeeId);

                if (existing != null)
                {
                    string oldVal = $"Ca: {existing.Shift}, Nghỉ: {existing.DayOff}";
                    string newVal = $"Ca: {item.Shift}, Nghỉ: {item.DayOff}";

                    if (existing.Shift != item.Shift || existing.DayOff != item.DayOff)
                    {
                        existing.Shift = item.Shift;
                        existing.DayOff = item.DayOff;
                        existing.UpdatedAt = DateTime.UtcNow;

                        // Log history
                        var history = new ScheduleHistory
                        {
                            Id = "HIS-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                            BranchId = branchId,
                            ModifiedById = managerId,
                            EmployeeId = item.EmployeeId,
                            Action = "Cập nhật ca trực",
                            OldValue = oldVal,
                            NewValue = newVal,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.ScheduleHistories.Add(history);
                    }
                }
                else
                {
                    var template = new EmployeeScheduleTemplate
                    {
                        Id = "TMP-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        EmployeeId = item.EmployeeId,
                        Shift = item.Shift,
                        DayOff = item.DayOff,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.EmployeeScheduleTemplates.Add(template);

                    // Log history
                    var history = new ScheduleHistory
                    {
                        Id = "HIS-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        BranchId = branchId,
                        ModifiedById = managerId,
                        EmployeeId = item.EmployeeId,
                        Action = "Phân ca trực mới",
                        OldValue = null,
                        NewValue = $"Ca: {item.Shift}, Nghỉ: {item.DayOff}",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.ScheduleHistories.Add(history);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ScheduleHistoryResponseDto>> GetScheduleHistoryAsync(string branchId)
        {
            var history = await _context.ScheduleHistories
                .Include(h => h.ModifiedBy)
                .Include(h => h.Employee)
                .Where(h => h.BranchId == branchId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return history.Select(h => new ScheduleHistoryResponseDto
            {
                Id = h.Id,
                CreatedAt = h.CreatedAt,
                ModifiedByFullName = h.ModifiedBy?.FullName ?? "Quản lý",
                Action = h.Action,
                EmployeeFullName = h.Employee?.FullName ?? "Nhân viên",
                OldValue = h.OldValue,
                NewValue = h.NewValue
            }).ToList();
        }

        public async Task<bool> UpdateEmployeeSalaryAsync(string employeeId, decimal salary)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId && !u.IsDeleted);
            if (user == null) return false;

            user.Salary = salary;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmployeeLeaveDaysAsync(string employeeId, int leaveDays)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId && !u.IsDeleted);
            if (user == null) return false;

            user.LeaveDays = leaveDays;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleEmployeeActiveAsync(string employeeId, bool isActive)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId && !u.IsDeleted);
            if (user == null) return false;

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
