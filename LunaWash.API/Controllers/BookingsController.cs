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

        //<<Comment Function>>
        // Hàm này là: Tạo mới một lịch đặt rửa xe khi người dùng thao tác đặt lịch
        //<</.....>>
        // [API: POST /api/Bookings, dùng để tạo lịch đặt mới, nhận thông tin đặt lịch từ FE]
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

        //<<Comment Function>>
        // Hàm này là: Lấy danh sách các khung giờ đã có người đặt
        //<</.....>>
        // [API: GET /api/Bookings/occupied-slots, dùng để lấy giờ đã bị chiếm, nhận date và washSlotId từ query]
        [HttpGet("occupied-slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOccupiedSlots([FromQuery] string date, [FromQuery] string washSlotId)
        {
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(washSlotId))
                return BadRequest("date and washSlotId are required.");
                
            var slots = await _bookingService.GetOccupiedSlotsAsync(date, washSlotId);
            return Ok(slots);
        }

        //<<Comment Function>>
        // Hàm này là: Lấy toàn bộ lịch sử các lượt đặt (cũ và hiện tại) của người dùng đang đăng nhập
        //<</.....>>
        // [API: GET /api/Bookings/history, dùng để lấy lịch sử đặt, yêu cầu token hợp lệ từ FE]
        [HttpGet("history")]
        public async Task<IActionResult> GetBookingHistory()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        //<<Comment Function>>
        // Hàm này là: Hủy một lịch đặt (xóa mềm, chuyển trạng thái thành Đã hủy)
        //<</.....>>
        // [API: DELETE /api/Bookings/{id}, dùng để hủy lịch, nhận id lịch đặt từ URL]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(string id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _bookingService.CancelBookingAsync(userId, id);
            if (!result) return NotFound(new { message = "Không tìm thấy lịch đặt hoặc không thể hủy." });

            return Ok(new { message = "Hủy lịch đặt thành công." });
        }

        //<<Comment Function>>
        // Hàm này là: Xóa vĩnh viễn một lịch đặt khỏi cơ sở dữ liệu
        //<</.....>>
        // [API: DELETE /api/Bookings/hard-delete/{id}, dùng để xóa hẳn lịch đặt, nhận id lịch từ URL]
        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> HardDeleteBooking(string id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _bookingService.HardDeleteBookingAsync(userId, id);
            if (!result) return NotFound(new { message = "Không tìm thấy lịch đặt để xóa." });

            return Ok(new { message = "Đã xóa bỏ lịch đặt hoàn toàn." });
        }

        //<<Comment Function>>
        // Hàm này là: Tìm kiếm các khung giờ trống cho một chi nhánh và ngày cụ thể
        //<</.....>>
        // [API: GET /api/Bookings/available-slots, dùng để lấy giờ trống, nhận branchId và date từ query]
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

        //<<Comment Function>>
        // Hàm này là: Yêu cầu khách hàng xác nhận đã đến nơi (được gọi bởi nhân viên)
        //<</.....>>
        // [API: PUT /api/Bookings/{id}/request-start, dùng để gửi yêu cầu xác nhận, nhận id từ URL]
        [HttpPut("{id}/request-start")]
        [AllowAnonymous] // Assuming staff authentication is handled or simplified for now
        public async Task<IActionResult> RequestStart(string id)
        {
            var result = await _bookingService.RequestCustomerConfirmationAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy lịch đặt." });
            return Ok(new { message = "Đã gửi yêu cầu xác nhận đến khách hàng." });
        }

        //<<Comment Function>>
        // Hàm này là: Khách hàng xác nhận họ đã đến nơi và sẵn sàng
        //<</.....>>
        // [API: PUT /api/Bookings/{id}/confirm-ready, dùng để xác nhận sẵn sàng, nhận id từ URL]
        [HttpPut("{id}/confirm-ready")]
        [AllowAnonymous] 
        public async Task<IActionResult> ConfirmReady(string id)
        {
            var result = await _bookingService.ConfirmReadyAsync(id);
            if (!result) return NotFound(new { message = "Không tìm thấy lịch đặt." });
            return Ok(new { message = "Đã xác nhận sẵn sàng." });
        }

        //<<Comment Function>>
        // Hàm này là: Lấy trạng thái xác nhận hiện tại của một lịch đặt
        //<</.....>>
        // [API: GET /api/Bookings/{id}/status, dùng để xem trạng thái xác nhận, nhận id từ URL]
        [HttpGet("{id}/status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetConfirmationStatus(string id)
        {
            var status = await _bookingService.GetBookingConfirmationStatusAsync(id);
            return Ok(new { isStartRequested = status.IsStartRequested, customerConfirmedReady = status.CustomerConfirmedReady });
        }
    }
}
