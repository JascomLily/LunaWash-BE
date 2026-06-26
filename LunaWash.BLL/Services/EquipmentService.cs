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
    public class EquipmentService : IEquipmentService
    {
        private readonly ApplicationDbContext _context;

        public EquipmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EquipmentDashboardDTO> GetDashboardAsync(string branchId)
        {
            var equipments = await _context.Equipments
                .Where(e => e.BranchId == branchId)
                .ToListAsync();

            var tasks = await _context.MaintenanceTasks
                .Where(t => t.BranchId == branchId)
                .ToListAsync();

            var result = new EquipmentDashboardDTO();

            foreach (var eq in equipments)
            {
                var eqDto = new EquipmentDTO
                {
                    Id = eq.Id,
                    Name = eq.Name,
                    Category = eq.Category,
                    Status = eq.Status,
                    LastMaintenance = eq.LastMaintenance,
                    NextMaintenance = eq.NextMaintenance,
                    Priority = eq.Priority,
                    StatusColor = GetStatusColor(eq.Status),
                    StatusIcon = GetStatusIcon(eq.Status),
                    PriorityColor = GetPriorityColor(eq.Priority),
                    NextMaintenanceColor = GetNextMaintenanceColor(eq.NextMaintenance)
                };
                result.Equipments.Add(eqDto);
            }

            foreach (var task in tasks)
            {
                var tDto = new MaintenanceTaskDTO
                {
                    Id = task.Id,
                    Name = task.TaskName,
                    Status = task.Status,
                    StatusColor = GetTaskStatusColor(task.Status),
                    Description = task.Description,
                    EquipmentId = task.EquipmentId,
                    EquipmentName = equipments.FirstOrDefault(e => e.Id == task.EquipmentId)?.Name ?? "Không xác định"
                };
                result.Tasks.Add(tDto);
            }

            return result;
        }

        public async Task<bool> UpdateEquipmentStatusAsync(string equipmentId, string branchId, string status)
        {
            var eq = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == equipmentId && e.BranchId == branchId);
            if (eq == null) return false;

            eq.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEquipmentScheduleAsync(string equipmentId, string branchId, string nextMaintenance)
        {
            var eq = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == equipmentId && e.BranchId == branchId);
            if (eq == null) return false;

            eq.NextMaintenance = nextMaintenance;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEquipmentPriorityAsync(string equipmentId, string branchId, string priority)
        {
            var eq = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == equipmentId && e.BranchId == branchId);
            if (eq == null) return false;

            eq.Priority = priority;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateIncidentTicketAsync(string branchId, CreateIncidentDTO dto)
        {
            var eq = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == dto.EquipmentId && e.BranchId == branchId);
            if (eq == null) return false;

            eq.Status = "Lỗi";
            eq.Priority = dto.Priority;

            var newTask = new MaintenanceTask
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                EquipmentId = eq.Id,
                BranchId = branchId,
                TaskName = $"Khắc phục: {eq.Name}",
                Description = dto.Description,
                Status = "Chưa làm"
            };

            _context.MaintenanceTasks.Add(newTask);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleTaskStatusAsync(string taskId, string branchId)
        {
            var task = await _context.MaintenanceTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.BranchId == branchId);
            if (task == null) return false;

            if (task.Status == "Chưa làm") task.Status = "Đang làm";
            else if (task.Status == "Đang làm") task.Status = "Hoàn thành";
            else if (task.Status == "Hoàn thành") task.Status = "Trễ hạn";
            else task.Status = "Chưa làm";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaskStatusAsync(string taskId, string branchId, string status)
        {
            var task = await _context.MaintenanceTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.BranchId == branchId);
            if (task == null) return false;

            task.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateReportAsync(string branchId, string equipmentId, string issueName, string description, string status, string taskStatus)
        {
            var eq = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == equipmentId && e.BranchId == branchId);
            if (eq == null) return false;

            // Cập nhật trạng thái thiết bị theo báo cáo
            eq.Status = status;

            var newTask = new MaintenanceTask
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(),
                EquipmentId = eq.Id,
                BranchId = branchId,
                TaskName = issueName,
                Description = description,
                Status = taskStatus
            };

            _context.MaintenanceTasks.Add(newTask);
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper methods for Colors
        private string GetStatusColor(string status)
        {
            if (status == "Hoạt động") return "text-emerald-700 bg-emerald-50 border-emerald-200";
            if (status == "Cần kiểm tra") return "text-amber-700 bg-amber-50 border-amber-200";
            if (status == "Đang bảo trì") return "text-sky-700 bg-sky-50 border-sky-200";
            if (status == "Lỗi") return "text-rose-700 bg-rose-50 border-rose-200";
            return "text-slate-700 bg-slate-50 border-slate-200";
        }

        private string GetStatusIcon(string status)
        {
            if (status == "Hoạt động") return "check_circle";
            if (status == "Cần kiểm tra") return "build";
            if (status == "Đang bảo trì") return "settings";
            if (status == "Lỗi") return "error";
            return "info";
        }

        private string GetPriorityColor(string priority)
        {
            if (priority == "Bình thường") return "text-slate-600 bg-slate-100";
            if (priority == "Trung bình") return "text-sky-700 bg-sky-100";
            if (priority == "Cao") return "text-blue-700 bg-blue-100";
            if (priority == "Khẩn cấp") return "text-rose-700 bg-rose-100";
            return "text-slate-600 bg-slate-100";
        }

        private string GetTaskStatusColor(string status)
        {
            if (status == "Chưa làm") return "text-slate-400";
            if (status == "Đang làm") return "text-blue-600";
            if (status == "Hoàn thành") return "text-emerald-600";
            if (status == "Trễ hạn") return "text-rose-600 font-bold";
            return "text-slate-400";
        }

        private string? GetNextMaintenanceColor(string? nextDate)
        {
            if (string.IsNullOrEmpty(nextDate)) return null;
            if (nextDate == "Hôm nay" || nextDate == "Ngay lập tức")
            {
                if (nextDate == "Hôm nay") return "text-amber-600 font-bold";
                if (nextDate == "Ngay lập tức") return "text-rose-600 font-bold";
            }
            return null;
        }
    }
}
