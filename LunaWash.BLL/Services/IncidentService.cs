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
    public class IncidentService : IIncidentService
    {
        private readonly ApplicationDbContext _context;

        public IncidentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IncidentReportResponseDto?> ReportIncidentAsync(string branchId, string reporterId, IncidentReportCreateDto dto)
        {
            var eq = await _context.Equipments.FirstOrDefaultAsync(e => e.Id == dto.EquipmentId && e.BranchId == branchId);
            if (eq == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == reporterId);
            if (user == null) return null;

            var id = "INC-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var incident = new IncidentReport
            {
                Id = id,
                BranchId = branchId,
                EquipmentId = dto.EquipmentId,
                ReporterId = reporterId,
                Title = dto.Title,
                Description = dto.Description,
                Status = "Chờ duyệt",
                CreatedAt = DateTime.UtcNow
            };

            _context.IncidentReports.Add(incident);
            await _context.SaveChangesAsync();

            return new IncidentReportResponseDto
            {
                Id = incident.Id,
                BranchId = incident.BranchId,
                EquipmentId = incident.EquipmentId,
                EquipmentName = eq.Name,
                ReporterId = incident.ReporterId,
                ReporterName = user.FullName,
                Title = incident.Title,
                Description = incident.Description,
                Status = incident.Status,
                CreatedAt = incident.CreatedAt
            };
        }

        public async Task<IEnumerable<IncidentReportResponseDto>> GetIncidentsByBranchAsync(string branchId)
        {
            var incidents = await _context.IncidentReports
                .Include(i => i.Equipment)
                .Include(i => i.Reporter)
                .Where(i => i.BranchId == branchId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return incidents.Select(i => new IncidentReportResponseDto
            {
                Id = i.Id,
                BranchId = i.BranchId,
                EquipmentId = i.EquipmentId,
                EquipmentName = i.Equipment.Name,
                ReporterId = i.ReporterId,
                ReporterName = i.Reporter.FullName,
                Title = i.Title,
                Description = i.Description,
                Status = i.Status,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            });
        }

        public async Task<bool> ApproveIncidentAsync(string id, string assignedToId, string priority)
        {
            var incident = await _context.IncidentReports.Include(i => i.Equipment).FirstOrDefaultAsync(i => i.Id == id);
            if (incident == null || incident.Status != "Chờ duyệt") return false;

            var tech = await _context.Users.FirstOrDefaultAsync(u => u.Id == assignedToId && u.RoleId == "ROL-05");
            if (tech == null) return false;

            // 1. Cập nhật trạng thái sự cố
            incident.Status = "Đã duyệt";
            incident.UpdatedAt = DateTime.UtcNow;

            // 2. Cập nhật trạng thái thiết bị thành Lỗi
            incident.Equipment.Status = "Lỗi";

            // 3. Tạo task sửa chữa phân công cho kỹ thuật
            var taskId = "T-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            var task = new MaintenanceTask
            {
                Id = taskId,
                BranchId = incident.BranchId,
                EquipmentId = incident.EquipmentId,
                TaskName = $"Sự cố: {incident.Title}",
                Description = incident.Description,
                Status = "Chưa làm",
                AssignedToId = assignedToId,
                IsIncident = true,
                Priority = priority,
                CreatedAt = DateTime.UtcNow
            };

            _context.MaintenanceTasks.Add(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectIncidentAsync(string id)
        {
            var incident = await _context.IncidentReports.FirstOrDefaultAsync(i => i.Id == id);
            if (incident == null || incident.Status != "Chờ duyệt") return false;

            incident.Status = "Đã từ chối";
            incident.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
