using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin payment settings
        /// </summary>
        [HttpGet("payments")]
        // BE: ĐÂY LÀ NƠI NHẬN API LẤY CẤU HÌNH THANH TOÁN
        // -> Được gọi từ: FE - src/pages/Admin/AdminTransactions.jsx (Khi Admin mở trang quản lý giao dịch)
        // -> Được gọi từ: FE - src/pages/Booking.jsx (Để kiểm tra xem VNPay/Tiền mặt có đang bật không và Hạng tối thiểu là gì)
        public async Task<IActionResult> GetPaymentSettings()
        {
            // Xuống tầng Service để lấy cấu hình thanh toán hiện tại từ file JSON hoặc DB
            var settings = await _settingsService.GetPaymentSettingsAsync();
            
            // Trả về dữ liệu cho FE
            return Ok(settings);
        }

        /// <summary>
        /// API xử lý chức năng: Cập nhật payment settings
        /// </summary>
        [HttpPut("payments")]
        // BE: ĐÂY LÀ NƠI NHẬN API ĐỂ LƯU CẤU HÌNH THANH TOÁN MỚI (Bật/tắt VNPay, Tiền mặt, Hạng tối thiểu)
        // -> Được gọi từ: FE - src/pages/Admin/AdminTransactions.jsx (Khi Admin bấm nút "Lưu")
        public async Task<IActionResult> UpdatePaymentSettings([FromBody] PaymentSettings settings)
        {
            // Nhận object settings từ FE gửi lên, truyền xuống tầng Service để xử lý lưu lại
            var result = await _settingsService.UpdatePaymentSettingsAsync(settings);
            
            // Nếu lưu thất bại, trả về lỗi 500
            if (!result) return StatusCode(500, "Lỗi khi lưu cấu hình thanh toán.");
            
            // Nếu lưu thành công, trả về trạng thái 200 OK cho FE biết
            return Ok(new { message = "Cập nhật cấu hình thanh toán thành công" });
        }
    }
}
