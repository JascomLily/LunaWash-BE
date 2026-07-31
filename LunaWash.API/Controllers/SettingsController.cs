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
        public async Task<IActionResult> GetPaymentSettings()
        {
            var settings = await _settingsService.GetPaymentSettingsAsync();
            return Ok(settings);
        }

        /// <summary>
        /// API xử lý chức năng: Cập nhật payment settings
        /// </summary>
        [HttpPut("payments")]
        public async Task<IActionResult> UpdatePaymentSettings([FromBody] PaymentSettings settings)
        {
            var result = await _settingsService.UpdatePaymentSettingsAsync(settings);
            if (!result) return StatusCode(500, "Lỗi khi lưu cấu hình thanh toán.");
            return Ok(new { message = "Cập nhật cấu hình thanh toán thành công" });
        }
    }
}
