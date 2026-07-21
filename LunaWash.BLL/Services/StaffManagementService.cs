using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using LunaWash.BLL.Interfaces;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Services
{
    public class StaffManagementService : IStaffManagementService
    {
        private readonly ApplicationDbContext _context;

        public StaffManagementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffScheduleDto>> GetSchedulesByBranchAsync(string branchId)
        {
            var schedules = await _context.StaffSchedules
                .Include(s => s.Employee)
                .Where(s => s.BranchId == branchId)
                .ToListAsync();

            return schedules.Select(s => new StaffScheduleDto
            {
                Id = s.EmployeeId, // map to Id so frontend's t.id mapping works
                EmployeeId = s.EmployeeId,
                FullName = s.Employee.FullName,
                Shift = s.Shift,
                DayOff = s.DayOff
            });
        }

        public async Task<bool> SaveSchedulesAsync(string branchId, string managerId, List<StaffScheduleDto> templates)
        {
            if (templates == null || !templates.Any()) return true;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingSchedules = await _context.StaffSchedules
                    .Where(s => s.BranchId == branchId)
                    .ToDictionaryAsync(s => s.EmployeeId);

                foreach (var dto in templates)
                {
                    var employeeExists = await _context.Users.AnyAsync(u => u.Id == dto.EmployeeId && u.BranchId == branchId);
                    if (!employeeExists) continue; // Skip if employee doesn't exist or is not in this branch

                    if (existingSchedules.TryGetValue(dto.EmployeeId, out var existing))
                    {
                        // Check for changes and log history
                        if (existing.Shift != dto.Shift)
                        {
                            var log = new ScheduleHistoryLog
                            {
                                Id = Guid.NewGuid().ToString(),
                                BranchId = branchId,
                                EmployeeId = dto.EmployeeId,
                                ModifiedById = managerId,
                                Action = "Cập nhật ca làm",
                                OldValue = existing.Shift,
                                NewValue = dto.Shift,
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.ScheduleHistoryLogs.Add(log);
                            existing.Shift = dto.Shift;
                        }

                        if (existing.DayOff != dto.DayOff)
                        {
                            var log = new ScheduleHistoryLog
                            {
                                Id = Guid.NewGuid().ToString(),
                                BranchId = branchId,
                                EmployeeId = dto.EmployeeId,
                                ModifiedById = managerId,
                                Action = "Cập nhật ngày nghỉ",
                                OldValue = existing.DayOff,
                                NewValue = dto.DayOff,
                                CreatedAt = DateTime.UtcNow
                            };
                            _context.ScheduleHistoryLogs.Add(log);
                            existing.DayOff = dto.DayOff;
                        }

                        existing.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // New schedule assignment
                        var newSchedule = new StaffSchedule
                        {
                            Id = Guid.NewGuid().ToString(),
                            EmployeeId = dto.EmployeeId,
                            BranchId = branchId,
                            Shift = dto.Shift,
                            DayOff = dto.DayOff,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.StaffSchedules.Add(newSchedule);

                        var log = new ScheduleHistoryLog
                        {
                            Id = Guid.NewGuid().ToString(),
                            BranchId = branchId,
                            EmployeeId = dto.EmployeeId,
                            ModifiedById = managerId,
                            Action = "Thiết lập lịch làm",
                            OldValue = null,
                            NewValue = $"{dto.Shift} / {dto.DayOff}",
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.ScheduleHistoryLogs.Add(log);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<IEnumerable<ScheduleHistoryLogDto>> GetHistoryByBranchAsync(string branchId)
        {
            var logs = await _context.ScheduleHistoryLogs
                .Include(l => l.Employee)
                .Include(l => l.ModifiedBy)
                .Where(l => l.BranchId == branchId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return logs.Select(l => new ScheduleHistoryLogDto
            {
                Id = l.Id,
                CreatedAt = l.CreatedAt,
                ModifiedByFullName = l.ModifiedBy.FullName,
                Action = l.Action,
                EmployeeFullName = l.Employee.FullName,
                OldValue = l.OldValue,
                NewValue = l.NewValue
            });
        }

        public async Task<bool> SaveAttendanceAsync(string branchId, string shift, List<AttendanceEntryDto> attendances)
        {
            if (attendances == null || !attendances.Any()) return true;

            var today = DateTime.UtcNow.Date;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var entry in attendances)
                {
                    var attendance = await _context.Attendances
                        .FirstOrDefaultAsync(a => a.UserId == entry.EmployeeId && a.AttendanceDate == today && a.BranchId == branchId);

                    if (attendance == null)
                    {
                        attendance = new Attendance
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = entry.EmployeeId,
                            BranchId = branchId,
                            AttendanceDate = today,
                            Status = entry.Status,
                            Note = entry.Note,
                            CheckInTime = entry.Status == "Present" || entry.Status == "Late" ? DateTime.UtcNow : null
                        };
                        _context.Attendances.Add(attendance);
                    }
                    else
                    {
                        attendance.Status = entry.Status;
                        attendance.Note = entry.Note;
                        if (entry.Status == "Present" || entry.Status == "Late")
                        {
                            if (attendance.CheckInTime == null)
                            {
                                attendance.CheckInTime = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            attendance.CheckInTime = null;
                            attendance.CheckOutTime = null;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
