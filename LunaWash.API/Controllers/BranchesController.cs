using System;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false)
        {
            try
            {
                var branches = await _branchService.GetAllBranchesAsync(activeOnly);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var branch = await _branchService.GetBranchByIdAsync(id);
                if (branch == null) return NotFound(new { message = "Không tìm thấy chi nhánh." });
                return Ok(branch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] BranchCreateDto dto)
        {
            try
            {
                var branch = await _branchService.CreateBranchAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = branch.Id }, branch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] BranchUpdateDto dto)
        {
            try
            {
                var branch = await _branchService.UpdateBranchAsync(id, dto);
                if (branch == null) return NotFound(new { message = "Không tìm thấy chi nhánh." });
                return Ok(branch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _branchService.DeleteBranchAsync(id);
                if (!success) return NotFound(new { message = "Không tìm thấy chi nhánh." });
                return Ok(new { message = "Xóa chi nhánh thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{branchId}/equipments")]
        public async Task<IActionResult> GetEquipments(string branchId)
        {
            try
            {
                var equipments = await _branchService.GetEquipmentsByBranchAsync(branchId);
                return Ok(equipments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("{branchId}/slots")]
        public async Task<IActionResult> GetSlots(string branchId)
        {
            try
            {
                var slots = await _branchService.GetSlotsByBranchAsync(branchId);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
