using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffManagementController : ControllerBase
    {
        private readonly IStaffManagementService _staffService;

        public StaffManagementController(IStaffManagementService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("branch/{branchId}/attendance")]
        [Authorize]
        public async Task<IActionResult> GetAttendance(string branchId, [FromQuery] string date, [FromQuery] string shift)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    parsedDate = DateTime.UtcNow;
                }

                var attendances = await _staffService.GetAttendanceAsync(branchId, parsedDate, shift);
                return Ok(attendances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("attendance")]
        [Authorize]
        public async Task<IActionResult> SaveAttendance([FromBody] SaveAttendanceDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var success = await _staffService.SaveAttendanceAsync(dto);
                if (!success) return BadRequest(new { message = "Không thể lưu điểm danh." });
                return Ok(new { message = "Lưu điểm danh thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("branch/{branchId}/templates")]
        [Authorize]
        public async Task<IActionResult> GetShiftTemplates(string branchId)
        {
            try
            {
                var templates = await _staffService.GetShiftTemplatesAsync(branchId);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("templates")]
        [Authorize]
        public async Task<IActionResult> SaveShiftTemplates([FromQuery] string branchId, [FromQuery] string managerId, [FromBody] SaveShiftTemplatesDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var success = await _staffService.SaveShiftTemplatesAsync(branchId, managerId, dto);
                if (!success) return BadRequest(new { message = "Không thể lưu khuôn mẫu lịch trực." });
                return Ok(new { message = "Lưu khuôn mẫu lịch trực thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("branch/{branchId}/history")]
        [Authorize]
        public async Task<IActionResult> GetScheduleHistory(string branchId)
        {
            try
            {
                var history = await _staffService.GetScheduleHistoryAsync(branchId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
