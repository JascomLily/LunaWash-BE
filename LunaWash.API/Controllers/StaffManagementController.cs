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

        // [API: Nhận yêu cầu GET từ FE để lấy lịch làm việc mẫu (templates) của nhân viên]
        //<<Comment Function>>
        // Hàm này là: Truy xuất dữ liệu lịch làm việc tiêu chuẩn của nhân viên thuộc chi nhánh.
        //<</.....>>
        [HttpGet("branch/{branchId}/templates")]
        public async Task<IActionResult> GetTemplates(string branchId)
        {
            var schedules = await _staffManagementService.GetSchedulesByBranchAsync(branchId);
            return Ok(schedules);
        }

        // [API: Nhận yêu cầu PUT từ FE để lưu lịch làm việc mẫu (templates) của nhân viên]
        //<<Comment Function>>
        // Hàm này là: Lưu thông tin thay đổi lịch làm việc mẫu của các nhân viên vào cơ sở dữ liệu.
        //<</.....>>
        [HttpPut("templates")]
        public async Task<IActionResult> SaveTemplates([FromQuery] string branchId, [FromQuery] string managerId, [FromBody] SaveStaffSchedulesRequest request)
        {
            var result = await _staffManagementService.SaveSchedulesAsync(branchId, managerId, request.Templates);
            if (!result) return BadRequest(new { message = "Lưu khuôn mẫu lịch thất bại." });
            return Ok(new { message = "Lưu khuôn mẫu lịch thành công." });
        }

        // [API: Nhận yêu cầu GET từ FE để lấy lịch sử cập nhật lịch làm việc của chi nhánh]
        //<<Comment Function>>
        // Hàm này là: Trả về nhật ký thay đổi lịch làm việc và ngày nghỉ của các nhân viên.
        //<</.....>>
        [HttpGet("branch/{branchId}/history")]
        public async Task<IActionResult> GetHistory(string branchId)
        {
            var history = await _staffManagementService.GetHistoryByBranchAsync(branchId);
            return Ok(history);
        }

        // [API: Nhận yêu cầu PUT từ FE để lưu thông tin điểm danh hàng loạt]
        //<<Comment Function>>
        // Hàm này là: Cập nhật và lưu lại trạng thái điểm danh (có mặt, vắng, đi trễ) của nhân viên.
        //<</.....>>
        [HttpPut("attendance")]
        public async Task<IActionResult> SaveAttendance([FromBody] SaveAttendanceRequest request)
        {
            var result = await _staffManagementService.SaveAttendanceAsync(request.BranchId, request.Shift, request.Attendances);
            if (!result) return BadRequest(new { message = "Lưu điểm danh thất bại." });
            return Ok(new { message = "Xác nhận điểm danh thành công." });
        }
    }
}
