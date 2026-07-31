using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        /// <summary>
        /// API xử lý chức năng: Tạo mới maintenance task
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> CreateMaintenanceTask([FromBody] CreateMaintenanceRequest request)
        {
            var result = await _maintenanceService.CreateMaintenanceTaskAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin maintenance tasks by branch
        /// </summary>
        [HttpGet("branch/{branchId}")]
        [Authorize(Roles = "Admin,BranchManager,TechnicalStaff")]
        public async Task<IActionResult> GetMaintenanceTasksByBranch(string branchId)
        {
            var result = await _maintenanceService.GetMaintenanceTasksByBranchAsync(branchId);
            return Ok(result);
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin maintenance tasks by assignee
        /// </summary>
        [HttpGet("assignee")]
        public async Task<IActionResult> GetMaintenanceTasksByAssignee()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _maintenanceService.GetMaintenanceTasksByAssigneeAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin maintenance task by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceTaskById(string id)
        {
            var result = await _maintenanceService.GetMaintenanceTaskByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// API xử lý chức năng: Cập nhật maintenance task status
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateMaintenanceTaskStatus(string id, [FromBody] UpdateMaintenanceStatusRequest request)
        {
            var success = await _maintenanceService.UpdateMaintenanceTaskStatusAsync(id, request);
            if (!success) return NotFound();
            return Ok(new { message = "Status updated successfully" });
        }

        /// <summary>
        /// API xử lý chức năng: Assign maintenance task
        /// </summary>
        [HttpPut("{id}/assign")]
        [Authorize(Roles = "TechnicalStaff")]
        public async Task<IActionResult> AssignMaintenanceTask(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _maintenanceService.AssignMaintenanceTaskAsync(id, userId);
            if (!result) return BadRequest(new { message = "Task cannot be assigned or is already claimed." });

            return NoContent();
        }
    }
}
