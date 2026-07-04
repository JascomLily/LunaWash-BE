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
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpPost]
        public async Task<IActionResult> ReportIncident([FromQuery] string branchId, [FromBody] IncidentReportCreateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Không xác định được danh tính người báo cáo. Vui lòng đăng nhập lại." });
                }

                var result = await _incidentService.ReportIncidentAsync(branchId, userId, dto);
                if (result == null)
                {
                    return BadRequest(new { message = "Thiết bị không tồn tại hoặc lỗi thông tin." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetIncidents(string branchId)
        {
            try
            {
                var incidents = await _incidentService.GetIncidentsByBranchAsync(branchId);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveIncident(string id, [FromBody] MaintenanceTaskAssignDto dto)
        {
            try
            {
                var success = await _incidentService.ApproveIncidentAsync(id, dto.AssignedToId, dto.Priority);
                if (!success)
                {
                    return BadRequest(new { message = "Không thể duyệt báo lỗi. Có thể do sự cố đã được xử lý hoặc thông tin kỹ thuật viên không hợp lệ." });
                }

                return Ok(new { message = "Duyệt báo cáo sự cố và tạo task giao việc thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectIncident(string id)
        {
            try
            {
                var success = await _incidentService.RejectIncidentAsync(id);
                if (!success)
                {
                    return BadRequest(new { message = "Không thể từ chối báo lỗi này." });
                }

                return Ok(new { message = "Đã từ chối báo cáo sự cố thành công." });
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
