using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBanners([FromQuery] bool activeOnly = false)
        {
            try
            {
                var banners = await _bannerService.GetAllBannersAsync(activeOnly);
                return Ok(banners);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBannerById(string id)
        {
            try
            {
                var banner = await _bannerService.GetBannerByIdAsync(id);
                if (banner == null) return NotFound(new { message = "Không tìm thấy banner." });
                return Ok(banner);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize] // Can restrict to Admin roles later
        public async Task<IActionResult> CreateBanner([FromBody] BannerCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var banner = await _bannerService.CreateBannerAsync(dto);
                return CreatedAtAction(nameof(GetBannerById), new { id = banner.Id }, banner);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBanner(string id, [FromBody] BannerCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var success = await _bannerService.UpdateBannerAsync(id, dto);
                if (!success) return NotFound(new { message = "Không tìm thấy banner." });
                return Ok(new { message = "Cập nhật banner thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBanner(string id)
        {
            try
            {
                var success = await _bannerService.DeleteBannerAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy banner." });
                return Ok(new { message = "Xóa banner thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("bulk")]
        [Authorize]
        public async Task<IActionResult> SaveBannersBulk([FromBody] IEnumerable<BannerCreateUpdateDto> dtos)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var success = await _bannerService.SaveBannersBulkAsync(dtos);
                if (!success) return BadRequest(new { message = "Không thể lưu danh sách banner." });
                return Ok(new { message = "Lưu danh sách banner thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
