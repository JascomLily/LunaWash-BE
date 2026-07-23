using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "BranchManager,Admin,Staff")] // Thêm phân quyền tùy nhu cầu
    public class EquipmentsController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;
        private readonly LunaWash.DAL.Data.ApplicationDbContext _context;

        public EquipmentsController(IEquipmentService equipmentService, LunaWash.DAL.Data.ApplicationDbContext context)
        {
            _equipmentService = equipmentService;
            _context = context;
        }

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetDashboard(string branchId, [FromQuery] string? washSlotId = null)
        {
            var result = await _equipmentService.GetDashboardAsync(branchId, washSlotId);
            return Ok(result);
        }

        [HttpGet("branch/{branchId}/slots")]
        public async Task<IActionResult> GetWashSlots(string branchId)
        {
            var slots = await _context.WashSlots
                .Where(s => s.BranchId == branchId && !s.IsDeleted)
                .OrderBy(s => s.SlotNumber)
                .Select(s => new {
                    id = s.Id,
                    name = "Trạm " + s.SlotNumber,
                    status = s.Status
                })
                .ToListAsync();
            return Ok(slots);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] string branchId, [FromBody] UpdateEquipmentStatusDTO dto)
        {
            var success = await _equipmentService.UpdateEquipmentStatusAsync(id, branchId, dto.Status);
            if (!success) return BadRequest(new { message = "Không thể cập nhật trạng thái thiết bị." });
            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }

        [HttpPut("{id}/schedule")]
        public async Task<IActionResult> UpdateSchedule(string id, [FromQuery] string branchId, [FromBody] UpdateEquipmentScheduleDTO dto)
        {
            var success = await _equipmentService.UpdateEquipmentScheduleAsync(id, branchId, dto.NextMaintenance);
            if (!success) return BadRequest(new { message = "Không thể cập nhật lịch bảo trì." });
            return Ok(new { message = "Cập nhật lịch thành công." });
        }

        [HttpPut("{id}/priority")]
        public async Task<IActionResult> UpdatePriority(string id, [FromQuery] string branchId, [FromBody] UpdateEquipmentPriorityDTO dto)
        {
            var success = await _equipmentService.UpdateEquipmentPriorityAsync(id, branchId, dto.Priority);
            if (!success) return BadRequest(new { message = "Không thể cập nhật mức độ ưu tiên." });
            return Ok(new { message = "Cập nhật ưu tiên thành công." });
        }

        [HttpPost("incidents")]
        public async Task<IActionResult> CreateIncident([FromQuery] string branchId, [FromBody] CreateIncidentDTO dto)
        {
            var success = await _equipmentService.CreateIncidentTicketAsync(branchId, dto);
            if (!success) return BadRequest(new { message = "Không thể tạo phiếu sự cố." });
            return Ok(new { message = "Tạo phiếu sự cố thành công." });
        }

        [HttpPost("{id}/report")]
        public async Task<IActionResult> CreateReport(string id, [FromQuery] string branchId, [FromBody] CreateReportDTO dto)
        {
            var success = await _equipmentService.CreateReportAsync(branchId, id, dto.IssueName, dto.Description, dto.Status, dto.TaskStatus);
            if (!success) return BadRequest(new { message = "Không thể gửi báo cáo." });
            return Ok(new { message = "Gửi báo cáo thành công." });
        }

        [HttpPut("tasks/{taskId}/toggle")]
        public async Task<IActionResult> ToggleTaskStatus(string taskId, [FromQuery] string branchId)
        {
            var success = await _equipmentService.ToggleTaskStatusAsync(taskId, branchId);
            if (!success) return BadRequest(new { message = "Không thể chuyển trạng thái công việc." });
            return Ok(new { message = "Đổi trạng thái công việc thành công." });
        }

        [HttpPut("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(string taskId, [FromQuery] string branchId, [FromBody] UpdateTaskStatusDTO dto)
        {
            var success = await _equipmentService.UpdateTaskStatusAsync(taskId, branchId, dto.Status);
            if (!success) return BadRequest(new { message = "Không thể cập nhật trạng thái báo cáo." });
            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed([FromQuery] string branchId, [FromServices] LunaWash.DAL.Data.ApplicationDbContext context)
        {
            // Clear existing data for this branch to generate a fresh professional demo
            var oldTasks = context.MaintenanceTasks.Where(t => t.BranchId == branchId).ToList();
            context.MaintenanceTasks.RemoveRange(oldTasks);
            await context.SaveChangesAsync(); // Delete tasks first because they reference Equipments and IncidentReports

            var oldIncidents = context.IncidentReports.Where(i => i.BranchId == branchId).ToList();
            context.IncidentReports.RemoveRange(oldIncidents);
            await context.SaveChangesAsync(); // Delete incidents next because they reference Equipments
            
            var oldEqs = context.Equipments.Where(e => e.BranchId == branchId).ToList();
            context.Equipments.RemoveRange(oldEqs);
            await context.SaveChangesAsync(); // Now we can safely delete equipments

            var eqs = new[] {
                new LunaWash.DAL.Entities.Equipment { Id = "EQ-001", BranchId = branchId, Name = "Máy rửa tự động 01", Category = "Máy rửa chính", Status = "Hoạt động", Priority = "Bình thường", LastMaintenance = "01/06/2026", NextMaintenance = "08/06/2026" },
                new LunaWash.DAL.Entities.Equipment { Id = "EQ-002", BranchId = branchId, Name = "Máy bơm áp lực cao", Category = "Bơm nước", Status = "Cần kiểm tra", Priority = "Cao", LastMaintenance = "30/05/2026", NextMaintenance = "Hôm nay" },
                new LunaWash.DAL.Entities.Equipment { Id = "EQ-003", BranchId = branchId, Name = "Cảm biến nhận diện xe", Category = "Cảm biến", Status = "Lỗi", Priority = "Khẩn cấp", LastMaintenance = "05/06/2026", NextMaintenance = "Ngay lập tức" },
                new LunaWash.DAL.Entities.Equipment { Id = "EQ-004", BranchId = branchId, Name = "Hệ thống phun bọt", Category = "Hóa chất", Status = "Đang bảo trì", Priority = "Trung bình", LastMaintenance = "03/06/2026", NextMaintenance = "10/06/2026" },
                new LunaWash.DAL.Entities.Equipment { Id = "EQ-005", BranchId = branchId, Name = "Máy sấy / quạt thổi khô", Category = "Sấy khô", Status = "Hoạt động", Priority = "Bình thường", LastMaintenance = "02/06/2026", NextMaintenance = "09/06/2026" }
            };
            context.Equipments.AddRange(eqs);
            await context.SaveChangesAsync();

            var tasks = new[] {
                new LunaWash.DAL.Entities.MaintenanceTask { Id = "T-001", BranchId = branchId, EquipmentId = "EQ-002", TaskName = "Kiểm tra máy bơm", Status = "Đang làm" },
                new LunaWash.DAL.Entities.MaintenanceTask { Id = "T-002", BranchId = branchId, EquipmentId = "EQ-004", TaskName = "Vệ sinh đầu phun", Status = "Chưa làm" },
                new LunaWash.DAL.Entities.MaintenanceTask { Id = "T-003", BranchId = branchId, EquipmentId = "EQ-003", TaskName = "Cảm biến nhận diện", Status = "Trễ hạn" },
                new LunaWash.DAL.Entities.MaintenanceTask { Id = "T-004", BranchId = branchId, EquipmentId = "EQ-001", TaskName = "Kiểm tra hóa chất", Status = "Hoàn thành" }
            };
            context.MaintenanceTasks.AddRange(tasks);
            await context.SaveChangesAsync();
            return Ok(new { message = "Seeded original demo data for branch " + branchId });
        }
    }
}
