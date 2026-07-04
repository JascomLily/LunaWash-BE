using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LunaWash.BLL.Services;

namespace LunaWash.API.Controllers
{
    [Route("api/settings")]
    [ApiController]
    [Authorize]
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingService _settingService;

        public SystemSettingsController(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSettings()
        {
            try
            {
                var settings = await _settingService.GetAllSettingsAsync();
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSetting(string id, [FromBody] UpdateSettingRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Value))
                {
                    return BadRequest(new { message = "Giá trị cấu hình không được để trống." });
                }

                var success = await _settingService.UpdateSettingAsync(id, request.Value);
                if (success)
                {
                    return Ok(new { message = $"Cập nhật cài đặt '{id}' thành công." });
                }
                return BadRequest(new { message = "Cập nhật cài đặt thất bại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class UpdateSettingRequest
    {
        public string Value { get; set; } = null!;
    }
}
