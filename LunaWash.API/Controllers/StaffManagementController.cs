using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;
using LunaWash.BLL.DTOs;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffManagementController : ControllerBase
    {
        private readonly IStaffManagementService _staffManagementService;

        public StaffManagementController(IStaffManagementService staffManagementService)
        {
            _staffManagementService = staffManagementService;
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin templates
        /// </summary>
        [HttpGet("branch/{branchId}/templates")]
        public async Task<IActionResult> GetTemplates(string branchId)
        {
            var schedules = await _staffManagementService.GetSchedulesByBranchAsync(branchId);
            return Ok(schedules);
        }

        /// <summary>
        /// API xử lý chức năng: Save templates
        /// </summary>
        [HttpPut("templates")]
        public async Task<IActionResult> SaveTemplates([FromQuery] string branchId, [FromQuery] string managerId, [FromBody] SaveStaffSchedulesRequest request)
        {
            var result = await _staffManagementService.SaveSchedulesAsync(branchId, managerId, request.Templates);
            if (!result) return BadRequest(new { message = "Lưu khuôn mẫu lịch thất bại." });
            return Ok(new { message = "Lưu khuôn mẫu lịch thành công." });
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin history
        /// </summary>
        [HttpGet("branch/{branchId}/history")]
        public async Task<IActionResult> GetHistory(string branchId)
        {
            var history = await _staffManagementService.GetHistoryByBranchAsync(branchId);
            return Ok(history);
        }

        /// <summary>
        /// API xử lý chức năng: Save attendance
        /// </summary>
        [HttpPut("attendance")]
        public async Task<IActionResult> SaveAttendance([FromBody] SaveAttendanceRequest request)
        {
            var result = await _staffManagementService.SaveAttendanceAsync(request.BranchId, request.Shift, request.Attendances);
            if (!result) return BadRequest(new { message = "Lưu điểm danh thất bại." });
            return Ok(new { message = "Xác nhận điểm danh thành công." });
        }
    }
}
