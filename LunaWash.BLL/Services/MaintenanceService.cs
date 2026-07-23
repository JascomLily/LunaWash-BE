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
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MaintenanceResponse> CreateMaintenanceTaskAsync(CreateMaintenanceRequest request)
        {
            var task = new MaintenanceTask
            {
                Id = Guid.NewGuid().ToString(),
                EquipmentId = request.EquipmentId,
                BranchId = request.BranchId,
                TaskName = request.TaskName,
                Description = request.Description,
                Priority = request.Priority,
                AssigneeId = request.AssigneeId,
                IncidentReportId = request.IncidentReportId,
                Status = "Chưa làm",
                CreatedAt = DateTime.UtcNow
            };

            _context.MaintenanceTasks.Add(task);

            if (!string.IsNullOrEmpty(request.IncidentReportId))
            {
                var incident = await _context.IncidentReports.FindAsync(request.IncidentReportId);
                if (incident != null)
                {
                    incident.Status = "Đang xử lý"; // Update incident status when assigned to a task
                }
            }
            if (string.IsNullOrEmpty(request.AssigneeId))
            {
                var techs = await _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.BranchId == request.BranchId && (u.Role.RoleName == "TechnicalStaff" || u.Role.RoleName == "Staff"))
                    .ToListAsync();
                
                foreach (var tech in techs)
                {
                    _context.Notifications.Add(new LunaWash.DAL.Entities.Notification
                    {
                        UserId = tech.Id,
                        Title = "Công việc mới",
                        Message = $"Có công việc bảo trì mới: {request.TaskName}",
                        Type = "info",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            return await GetMaintenanceTaskByIdAsync(task.Id) ?? throw new Exception("Failed to create maintenance task");
        }

        public async Task<IEnumerable<MaintenanceResponse>> GetMaintenanceTasksByBranchAsync(string branchId)
        {
            return await _context.MaintenanceTasks
                .Include(m => m.Equipment)
                .Include(m => m.Branch)
                .Include(m => m.Assignee)
                .Where(m => m.BranchId == branchId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => MapToResponse(m))
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceResponse>> GetMaintenanceTasksByAssigneeAsync(string assigneeId)
        {
            return await _context.MaintenanceTasks
                .Include(m => m.Equipment)
                .Include(m => m.Branch)
                .Include(m => m.Assignee)
                .Where(m => m.AssigneeId == assigneeId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => MapToResponse(m))
                .ToListAsync();
        }

        public async Task<MaintenanceResponse?> GetMaintenanceTaskByIdAsync(string taskId)
        {
            var task = await _context.MaintenanceTasks
                .Include(m => m.Equipment)
                .Include(m => m.Branch)
                .Include(m => m.Assignee)
                .FirstOrDefaultAsync(m => m.Id == taskId);

            return task != null ? MapToResponse(task) : null;
        }

        public async Task<bool> UpdateMaintenanceTaskStatusAsync(string taskId, UpdateMaintenanceStatusRequest request)
        {
            var task = await _context.MaintenanceTasks
                .Include(m => m.IncidentReport)
                .FirstOrDefaultAsync(m => m.Id == taskId);
                
            if (task == null) return false;

            task.Status = request.Status;
            
            if (request.ReviewNote != null)
            {
                task.ReviewNote = request.ReviewNote;
            }

            if (request.Status == "Chờ nghiệm thu")
            {
                var manager = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.BranchId == task.BranchId && u.Role.RoleName == "BranchManager");
                
                if (manager != null)
                {
                    _context.Notifications.Add(new LunaWash.DAL.Entities.Notification
                    {
                        UserId = manager.Id,
                        Title = "Nghiệm thu sự cố",
                        Message = $"Sự cố '{task.TaskName}' đang chờ bạn nghiệm thu.",
                        Type = "incident",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (request.Status == "Hoàn thành")
            {
                if (task.IncidentReportId != null && task.IncidentReport != null)
                {
                    task.IncidentReport.Status = "Đã giải quyết";
                }
                
                // Update equipment status back to Tốt and set LastMaintenance
                if (!string.IsNullOrEmpty(task.EquipmentId))
                {
                    var eq = await _context.Equipments.FindAsync(task.EquipmentId);
                    if (eq != null)
                    {
                        eq.Status = "Tốt";
                        eq.LastMaintenance = DateTime.UtcNow.ToString("o");
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignMaintenanceTaskAsync(string taskId, string assigneeId)
        {
            var task = await _context.MaintenanceTasks.FindAsync(taskId);
            if (task == null) return false;

            if (!string.IsNullOrEmpty(task.AssigneeId))
            {
                // Đã có người nhận
                return false;
            }

            task.AssigneeId = assigneeId;
            task.Status = "Đang làm";
            
            // Cập nhật trạng thái thiết bị thành "Đang bảo trì"
            if (!string.IsNullOrEmpty(task.EquipmentId))
            {
                var eq = await _context.Equipments.FindAsync(task.EquipmentId);
                if (eq != null)
                {
                    eq.Status = "Đang bảo trì";
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static MaintenanceResponse MapToResponse(MaintenanceTask m)
        {
            return new MaintenanceResponse
            {
                Id = m.Id,
                EquipmentId = m.EquipmentId,
                EquipmentName = m.Equipment?.Name ?? "N/A",
                BranchId = m.BranchId,
                BranchName = m.Branch.BranchName,
                TaskName = m.TaskName,
                Description = m.Description,
                Status = m.Status,
                Priority = m.Priority,
                ReviewNote = m.ReviewNote,
                AssigneeId = m.AssigneeId,
                AssigneeName = m.Assignee?.FullName,
                IncidentReportId = m.IncidentReportId,
                CreatedAt = m.CreatedAt
            };
        }
    }
}
