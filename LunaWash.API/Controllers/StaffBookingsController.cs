using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunaWash.API.Controllers
{
    [Route("api/staff/bookings")]
    [ApiController]
    [Authorize] // Có thể thêm [Authorize(Roles = "Staff, Admin")] nếu Role setup chuẩn
    public class StaffBookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public StaffBookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        
        // [API: Nhận yêu cầu GET từ FE để lấy danh sách lịch đặt (queue) trong ngày của chi nhánh]
        //<<Comment Function>>
        // Hàm này là: Lấy danh sách các đơn đặt dịch vụ hôm nay của chi nhánh, có thể lọc theo ngày.
        //<</.....>>
        [HttpGet("today/{branchId}")]
        public async Task<IActionResult> GetTodayQueue(string branchId, [FromQuery] string? date)
        {
            var bookings = await _bookingService.GetTodayBookingsForStaffAsync(branchId, date);
            return Ok(bookings);
        }

        // [API: Nhận yêu cầu GET từ FE để lấy lịch sử đơn hàng của chi nhánh]
        //<<Comment Function>>
        // Hàm này là: Lấy danh sách lịch sử tất cả các lịch đặt dịch vụ của một chi nhánh cụ thể.
        //<</.....>>
        [HttpGet("history/{branchId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBranchHistory(string branchId)
        {
            var bookings = await _bookingService.GetBranchHistoryAsync(branchId);
            return Ok(bookings);
        }

      
        // [API: Nhận yêu cầu PUT từ FE để cập nhật trạng thái đơn đặt dịch vụ]
        //<<Comment Function>>
        // Hàm này là: Cập nhật trạng thái của một lịch đặt dịch vụ (ví dụ: hoàn thành, hủy).
        //<</.....>>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateBookingStatusDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Status)) return BadRequest("Status is required.");

            var result = await _bookingService.UpdateBookingStatusAsync(id, dto.Status);
            if (!result) return NotFound(new { message = "Không tìm thấy lịch đặt." });

            return Ok(new { message = $"Đã cập nhật trạng thái thành: {dto.Status}" });
        }

        // [API: Nhận yêu cầu PUT từ FE để thêm dịch vụ dọn nội thất vào đơn đặt dịch vụ]
        //<<Comment Function>>
        // Hàm này là: Thêm dịch vụ dọn nội thất vào một lịch đặt hiện có.
        //<</.....>>
        [HttpPut("{id}/add-interior-cleaning")]
        public async Task<IActionResult> AddInteriorCleaning(string id)
        {
            var (success, message) = await _bookingService.AddInteriorCleaningAsync(id);
            if (!success) return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}