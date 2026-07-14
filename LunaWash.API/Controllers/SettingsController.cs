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

        [HttpGet("payments")]
        public async Task<IActionResult> GetPaymentSettings()
        {
            var settings = await _settingsService.GetPaymentSettingsAsync();
            return Ok(settings);
        }

        [HttpPut("payments")]
        public async Task<IActionResult> UpdatePaymentSettings([FromBody] PaymentSettings settings)
        {
            var result = await _settingsService.UpdatePaymentSettingsAsync(settings);
            if (!result) return StatusCode(500, "Lỗi khi lưu cấu hình thanh toán.");
            return Ok(new { message = "Cập nhật cấu hình thanh toán thành công" });
        }
    }
}
