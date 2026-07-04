using System;
using System.Security.Claims;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LunaWash.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet("tasks/technician/{techId}")]
        public async Task<IActionResult> GetTechTasks(string techId)
        {
            try
            {
                var tasks = await _maintenanceService.GetTasksByTechnicianAsync(techId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("tasks/branch/{branchId}")]
        public async Task<IActionResult> GetBranchTasks(string branchId)
        {
            try
            {
                var tasks = await _maintenanceService.GetTasksByBranchAsync(branchId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("tasks/{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(string id, [FromBody] MaintenanceTaskUpdateStatusDto dto)
        {
            try
            {
                var techId = GetCurrentUserId();
                if (string.IsNullOrEmpty(techId))
                {
                    return Unauthorized(new { message = "Không xác định được danh tính kỹ thuật viên." });
                }

                var success = await _maintenanceService.UpdateTaskStatusAsync(id, techId, dto);
                if (!success)
                {
                    return BadRequest(new { message = "Không thể cập nhật trạng thái nhiệm vụ." });
                }

                return Ok(new { message = "Cập nhật trạng thái nhiệm vụ thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("tasks/{id}/confirm")]
        public async Task<IActionResult> ConfirmTaskCompletion(string id)
        {
            try
            {
                var success = await _maintenanceService.ConfirmTaskCompletionAsync(id);
                if (!success)
                {
                    return BadRequest(new { message = "Nghiệm thu công việc thất bại." });
                }

                return Ok(new { message = "Nghiệm thu và xác nhận hoàn thành công việc sửa chữa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("tasks/{id}/assign")]
        public async Task<IActionResult> AssignTask(string id, [FromBody] MaintenanceTaskAssignDto dto)
        {
            try
            {
                var success = await _maintenanceService.AssignTaskAsync(id, dto.AssignedToId, dto.Priority);
                if (!success)
                {
                    return BadRequest(new { message = "Giao việc sửa chữa thất bại." });
                }

                return Ok(new { message = "Giao việc sửa chữa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("check-log")]
        public async Task<IActionResult> CreateCheckLog([FromQuery] string branchId, [FromBody] EquipmentCheckLogCreateDto dto)
        {
            try
            {
                var techId = GetCurrentUserId();
                if (string.IsNullOrEmpty(techId))
                {
                    return Unauthorized(new { message = "Không xác định được danh tính kỹ thuật viên." });
                }

                var result = await _maintenanceService.CreateCheckLogAsync(branchId, techId, dto);
                if (result == null)
                {
                    return BadRequest(new { message = "Không thể gửi nhật ký ca trực định kỳ." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("check-log/branch/{branchId}")]
        public async Task<IActionResult> GetCheckLogs(string branchId)
        {
            try
            {
                var logs = await _maintenanceService.GetCheckLogsByBranchAsync(branchId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId) && Request.Headers.TryGetValue("X-User-Id", out var headerVal))
            {
                userId = headerVal.ToString();
            }

            return userId ?? string.Empty;
        }
    }
}
