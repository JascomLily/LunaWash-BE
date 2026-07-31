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

        
        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin today queue
        /// </summary>
        [HttpGet("today/{branchId}")]
        public async Task<IActionResult> GetTodayQueue(string branchId, [FromQuery] string? date)
        {
            var bookings = await _bookingService.GetTodayBookingsForStaffAsync(branchId, date);
            return Ok(bookings);
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin branch history
        /// </summary>
        [HttpGet("history/{branchId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBranchHistory(string branchId)
        {
            var bookings = await _bookingService.GetBranchHistoryAsync(branchId);
            return Ok(bookings);
        }

      
        /// <summary>
        /// API xử lý chức năng: Cập nhật status
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateBookingStatusDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Status)) return BadRequest("Status is required.");

            var result = await _bookingService.UpdateBookingStatusAsync(id, dto.Status);
            if (!result) return NotFound(new { message = "Không tìm thấy lịch đặt." });

            return Ok(new { message = $"Đã cập nhật trạng thái thành: {dto.Status}" });
        }

        /// <summary>
        /// API xử lý chức năng: Thêm interior cleaning
        /// </summary>
        [HttpPut("{id}/add-interior-cleaning")]
        public async Task<IActionResult> AddInteriorCleaning(string id)
        {
            var (success, message) = await _bookingService.AddInteriorCleaningAsync(id);
            if (!success) return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}