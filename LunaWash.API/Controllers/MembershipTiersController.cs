using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipTiersController : ControllerBase
    {
        private readonly IMembershipTierService _tierService;

        public MembershipTiersController(IMembershipTierService tierService)
        {
            _tierService = tierService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTiers()
        {
            try
            {
                var tiers = await _tierService.GetAllTiersAsync();
                return Ok(tiers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTierById(string id)
        {
            try
            {
                var tier = await _tierService.GetTierByIdAsync(id);
                if (tier == null) return NotFound(new { message = "Không tìm thấy hạng thành viên." });
                return Ok(tier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("{customId}")]
        [Authorize]
        public async Task<IActionResult> CreateTier(string customId, [FromBody] MembershipTierCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var tier = await _tierService.CreateTierAsync(customId, dto);
                return CreatedAtAction(nameof(GetTierById), new { id = tier.Id }, tier);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTier(string id, [FromBody] MembershipTierCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var success = await _tierService.UpdateTierAsync(id, dto);
                if (!success) return NotFound(new { message = "Không tìm thấy hạng thành viên hoặc hạng đã bị xóa." });
                return Ok(new { message = "Cập nhật hạng thành viên thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTier(string id)
        {
            try
            {
                var success = await _tierService.DeleteTierAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy hạng thành viên hoặc hạng đã bị xóa." });
                return Ok(new { message = "Xóa hạng thành viên thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("adjust-points")]
        [Authorize]
        public async Task<IActionResult> AdjustPoints([FromBody] AdjustPointsDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var success = await _tierService.AdjustCustomerPointsAsync(dto);
                if (!success) return NotFound(new { message = "Không tìm thấy hồ sơ khách hàng để điều chỉnh điểm." });
                return Ok(new { message = "Điều chỉnh điểm tích lũy thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetPointHistory(string userId)
        {
            try
            {
                var history = await _tierService.GetCustomerPointHistoryAsync(userId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
