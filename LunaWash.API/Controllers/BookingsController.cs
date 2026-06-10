using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Services;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirstValue("sub") ?? User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) userId = "USR-2606-79E0"; 

            try
            {
                var booking = await _bookingService.CreateBookingAsync(userId, dto);
                return Ok(booking);
            }
            catch (InvalidOperationException ex)
            {
                // Bắt lỗi Hết slot và thông báo cho người dùng
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình đặt lịch. Vui lòng thử lại sau.", details = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetBookingHistory()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(string id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _bookingService.CancelBookingAsync(userId, id);
            if (!result) return NotFound(new { message = "Không tìm thấy lịch đặt hoặc không thể hủy." });

            return Ok(new { message = "Hủy lịch đặt thành công." });
        }

        [HttpGet("available-slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] string branchId, [FromQuery] string date)
        {
            if (string.IsNullOrEmpty(branchId) || string.IsNullOrEmpty(date))
                return BadRequest(new { message = "Thiếu thông tin branchId hoặc date (yyyy-MM-dd)." });

            if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
                return BadRequest(new { message = "Định dạng ngày không hợp lệ. Vui lòng dùng yyyy-MM-dd." });

            try
            {
                var slots = await _bookingService.GetAvailableTimeSlotsAsync(branchId, parsedDate);
                return Ok(slots);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách giờ trống.", details = ex.Message });
            }
        }
    }
}
