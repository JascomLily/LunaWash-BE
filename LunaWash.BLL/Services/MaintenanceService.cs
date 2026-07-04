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
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaintenanceTaskDetailDto>> GetTasksByTechnicianAsync(string techId)
        {
            var tasks = await _context.MaintenanceTasks
                .Include(t => t.Equipment)
                .Include(t => t.AssignedTo)
                .Where(t => t.AssignedToId == techId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(t => new MaintenanceTaskDetailDto
            {
                Id = t.Id,
                BranchId = t.BranchId,
                EquipmentId = t.EquipmentId,
                EquipmentName = t.Equipment.Name,
                TaskName = t.TaskName,
                Description = t.Description,
                Status = t.Status,
                AssignedToId = t.AssignedToId,
                AssignedToName = t.AssignedTo?.FullName,
                Resolution = t.Resolution,
                SupportRequest = t.SupportRequest,
                IsIncident = t.IsIncident,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            });
        }

        public async Task<IEnumerable<MaintenanceTaskDetailDto>> GetTasksByBranchAsync(string branchId)
        {
            var tasks = await _context.MaintenanceTasks
                .Include(t => t.Equipment)
                .Include(t => t.AssignedTo)
                .Where(t => t.BranchId == branchId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(t => new MaintenanceTaskDetailDto
            {
                Id = t.Id,
                BranchId = t.BranchId,
                EquipmentId = t.EquipmentId,
                EquipmentName = t.Equipment.Name,
                TaskName = t.TaskName,
                Description = t.Description,
                Status = t.Status,
                AssignedToId = t.AssignedToId,
                AssignedToName = t.AssignedTo?.FullName,
                Resolution = t.Resolution,
                SupportRequest = t.SupportRequest,
                IsIncident = t.IsIncident,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            });
        }

        public async Task<bool> UpdateTaskStatusAsync(string id, string techId, MaintenanceTaskUpdateStatusDto dto)
        {
            var task = await _context.MaintenanceTasks.Include(t => t.Equipment).FirstOrDefaultAsync(t => t.Id == id && t.AssignedToId == techId);
            if (task == null) return false;

            task.Status = dto.Status;
            task.UpdatedAt = DateTime.UtcNow;

            if (dto.Status == "Hoàn thành")
            {
                task.Resolution = dto.Resolution;
                task.SupportRequest = dto.SupportRequest;
                // Khi hoàn thành sửa chữa, đưa thiết bị về "Cần kiểm tra" để Quản lý xuống nghiệm thu
                task.Equipment.Status = "Cần kiểm tra";
            }
            else if (dto.Status == "Chờ kinh phí" || dto.Status == "Đang làm")
            {
                task.SupportRequest = dto.SupportRequest;
                task.Equipment.Status = "Đang bảo trì";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmTaskCompletionAsync(string id)
        {
            var task = await _context.MaintenanceTasks.Include(t => t.Equipment).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return false;

            task.Status = "Đã nghiệm thu";
            task.UpdatedAt = DateTime.UtcNow;

            // Đưa thiết bị về trạng thái Hoạt động sau khi nghiệm thu
            task.Equipment.Status = "Hoạt động";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignTaskAsync(string id, string assignedToId, string priority)
        {
            var task = await _context.MaintenanceTasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return false;

            var tech = await _context.Users.FirstOrDefaultAsync(u => u.Id == assignedToId && u.RoleId == "ROL-05");
            if (tech == null) return false;

            task.AssignedToId = assignedToId;
            task.Priority = priority;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EquipmentCheckLogResponseDto?> CreateCheckLogAsync(string branchId, string techId, EquipmentCheckLogCreateDto dto)
        {
            var eq = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == dto.EquipmentId && e.BranchId == branchId);
            if (eq == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == techId && u.RoleId == "ROL-05");
            if (user == null) return null;

            var id = "LOG-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var log = new EquipmentCheckLog
            {
                Id = id,
                BranchId = branchId,
                EquipmentId = dto.EquipmentId,
                TechnicianId = techId,
                CheckTime = DateTime.UtcNow,
                Condition = dto.Condition,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            // Tự động đồng bộ trạng thái thiết bị theo kết quả kiểm tra
            if (dto.Condition == "Lỗi")
            {
                eq.Status = "Lỗi";
            }
            else if (dto.Condition == "Cần kiểm tra")
            {
                eq.Status = "Cần kiểm tra";
            }
            else
            {
                eq.Status = "Hoạt động";
            }

            _context.EquipmentCheckLogs.Add(log);
            await _context.SaveChangesAsync();

            return new EquipmentCheckLogResponseDto
            {
                Id = log.Id,
                BranchId = log.BranchId,
                EquipmentId = log.EquipmentId,
                EquipmentName = eq.Name,
                TechnicianId = log.TechnicianId,
                TechnicianName = user.FullName,
                CheckTime = log.CheckTime,
                Condition = log.Condition,
                Notes = log.Notes,
                CreatedAt = log.CreatedAt
            };
        }

        public async Task<IEnumerable<EquipmentCheckLogResponseDto>> GetCheckLogsByBranchAsync(string branchId)
        {
            var logs = await _context.EquipmentCheckLogs
                .Include(l => l.Equipment)
                .Include(l => l.Technician)
                .Where(l => l.BranchId == branchId)
                .OrderByDescending(l => l.CheckTime)
                .ToListAsync();

            return logs.Select(l => new EquipmentCheckLogResponseDto
            {
                Id = l.Id,
                BranchId = l.BranchId,
                EquipmentId = l.EquipmentId,
                EquipmentName = l.Equipment.Name,
                TechnicianId = l.TechnicianId,
                TechnicianName = l.Technician.FullName,
                CheckTime = l.CheckTime,
                Condition = l.Condition,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt
            });
        }
    }
}
