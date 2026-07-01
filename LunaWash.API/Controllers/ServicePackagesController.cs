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
    public class ServicePackagesController : ControllerBase
    {
        private readonly IServicePackageService _packageService;

        public ServicePackagesController(IServicePackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPackages([FromQuery] bool activeOnly = false)
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync(activeOnly);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPackageById(string id)
        {
            try
            {
                var package = await _packageService.GetPackageByIdAsync(id);
                if (package == null) return NotFound(new { message = "Không tìm thấy gói dịch vụ." });
                return Ok(package);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize] // Có thể phân quyền [Authorize(Roles = "Admin")] sau này
        public async Task<IActionResult> CreatePackage([FromBody] ServicePackageCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var package = await _packageService.CreatePackageAsync(dto);
                return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePackage(string id, [FromBody] ServicePackageCreateUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var success = await _packageService.UpdatePackageAsync(id, dto);
                if (!success) return NotFound(new { message = "Không tìm thấy gói dịch vụ hoặc gói đã bị xóa." });
                return Ok(new { message = "Cập nhật gói dịch vụ thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePackage(string id)
        {
            try
            {
                var success = await _packageService.DeletePackageAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy gói dịch vụ hoặc gói đã bị xóa." });
                return Ok(new { message = "Xóa gói dịch vụ thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
