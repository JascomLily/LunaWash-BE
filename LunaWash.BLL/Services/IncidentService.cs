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
    public class IncidentService : IIncidentService
    {
        private readonly ApplicationDbContext _context;

        public IncidentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IncidentResponse> CreateIncidentAsync(CreateIncidentRequest request, string reporterId)
        {
            var incident = new IncidentReport
            {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title,
                EquipmentId = request.EquipmentId,
                BranchId = request.BranchId,
                ReporterId = reporterId,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                Status = "Chờ xử lý",
                CreatedAt = DateTime.UtcNow
            };

            _context.IncidentReports.Add(incident);

            // Bắn thông báo cho Quản lý của chi nhánh
            var managers = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.BranchId == request.BranchId && (u.Role.RoleName == "BranchManager" || u.Role.RoleName == "Admin"))
                .ToListAsync();

            foreach (var manager in managers)
            {
                _context.Notifications.Add(new LunaWash.DAL.Entities.Notification
                {
                    UserId = manager.Id,
                    Title = "Báo cáo sự cố mới",
                    Message = $"Sự cố mới: {request.Title}",
                    Type = "System"
                });
            }

            // Cập nhật trạng thái thiết bị thành "Đang hỏng"
            if (!string.IsNullOrEmpty(request.EquipmentId))
            {
                var eq = await _context.Equipments.FindAsync(request.EquipmentId);
                if (eq != null)
                {
                    eq.Status = "Đang hỏng";
                }
            }

            await _context.SaveChangesAsync();

            return await GetIncidentByIdAsync(incident.Id) ?? throw new Exception("Failed to create incident");
        }

        public async Task<IEnumerable<IncidentResponse>> GetIncidentsByBranchAsync(string branchId)
        {
            return await _context.IncidentReports
                .Include(i => i.Reporter)
                .Include(i => i.Equipment)
                .Include(i => i.Branch)
                .Where(i => i.BranchId == branchId)
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new IncidentResponse
                {
                    Id = i.Id,
                    Title = i.Title,
                    EquipmentId = i.EquipmentId,
                    EquipmentName = i.Equipment != null ? i.Equipment.Name : null,
                    BranchId = i.BranchId,
                    BranchName = i.Branch.BranchName,
                    ReporterId = i.ReporterId,
                    ReporterName = i.Reporter.FullName,
                    Description = i.Description,
                    Status = i.Status,
                    ImageUrl = i.ImageUrl,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IncidentResponse?> GetIncidentByIdAsync(string incidentId)
        {
            var incident = await _context.IncidentReports
                .Include(i => i.Reporter)
                .Include(i => i.Equipment)
                .Include(i => i.Branch)
                .FirstOrDefaultAsync(i => i.Id == incidentId);

            if (incident == null) return null;

            return new IncidentResponse
            {
                Id = incident.Id,
                Title = incident.Title,
                EquipmentId = incident.EquipmentId,
                EquipmentName = incident.Equipment?.Name,
                BranchId = incident.BranchId,
                BranchName = incident.Branch.BranchName,
                ReporterId = incident.ReporterId,
                ReporterName = incident.Reporter.FullName,
                Description = incident.Description,
                Status = incident.Status,
                ImageUrl = incident.ImageUrl,
                CreatedAt = incident.CreatedAt
            };
        }

        public async Task<bool> UpdateIncidentStatusAsync(string incidentId, string status)
        {
            var incident = await _context.IncidentReports.FindAsync(incidentId);
            if (incident == null) return false;

            incident.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
