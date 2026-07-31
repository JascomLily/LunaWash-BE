using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin settings
        /// </summary>
        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var tiers = await _membershipService.GetAllTiersAsync();
            return Ok(tiers);
        }

        /// <summary>
        /// API xử lý chức năng: Cập nhật setting
        /// </summary>
        [HttpPut("settings/{id}")]
        public async Task<IActionResult> UpdateSetting(string id, [FromBody] MembershipTierUpdateDto dto)
        {
            var result = await _membershipService.UpdateTierAsync(id, dto);
            if (!result) return NotFound("Không tìm thấy hạng thành viên.");
            return Ok(new { message = "Cập nhật hạng thành viên thành công" });
        }

        /// <summary>
        /// API xử lý chức năng: Sync customer tiers
        /// </summary>
        [HttpPost("sync")]
        public async Task<IActionResult> SyncCustomerTiers()
        {
            var result = await _membershipService.SyncCustomerTiersAsync();
            if (!result) return BadRequest("Lỗi khi đồng bộ khách hàng.");
            return Ok(new { message = "Đồng bộ thành công" });
        }
    }
}
