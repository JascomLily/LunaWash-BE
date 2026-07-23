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

        [HttpPost]
        [Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> CreateMaintenanceTask([FromBody] CreateMaintenanceRequest request)
        {
            var result = await _maintenanceService.CreateMaintenanceTaskAsync(request);
            return Ok(result);
        }

        [HttpGet("branch/{branchId}")]
        [Authorize(Roles = "Admin,BranchManager,TechnicalStaff")]
        public async Task<IActionResult> GetMaintenanceTasksByBranch(string branchId)
        {
            var result = await _maintenanceService.GetMaintenanceTasksByBranchAsync(branchId);
            return Ok(result);
        }

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceTaskById(string id)
        {
            var result = await _maintenanceService.GetMaintenanceTaskByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateMaintenanceTaskStatus(string id, [FromBody] UpdateMaintenanceStatusRequest request)
        {
            var success = await _maintenanceService.UpdateMaintenanceTaskStatusAsync(id, request);
            if (!success) return NotFound();
            return Ok(new { message = "Status updated successfully" });
        }

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
