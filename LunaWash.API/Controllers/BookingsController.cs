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
            if (string.IsNullOrEmpty(userId))
            {
                userId = "USR-2606-79E0"; // Fallback cho việc test qua curl không có token
            }

            var booking = await _bookingService.CreateBookingAsync(userId, dto);
            if (booking == null)
            {
                return BadRequest(new { message = "Không thể tạo lịch đặt. Vui lòng kiểm tra lại thông tin xe hoặc lịch trống." });
            }

            return Ok(booking);
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
    }
}
