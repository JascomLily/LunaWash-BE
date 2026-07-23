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
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentService _incidentService;

        public IncidentsController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _incidentService.CreateIncidentAsync(request, userId);
            return Ok(result);
        }

        [HttpGet("branch/{branchId}")]
        [Authorize(Roles = "Admin,BranchManager,Staff")]
        public async Task<IActionResult> GetIncidentsByBranch(string branchId)
        {
            var result = await _incidentService.GetIncidentsByBranchAsync(branchId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncidentById(string id)
        {
            var result = await _incidentService.GetIncidentByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,BranchManager")]
        public async Task<IActionResult> UpdateIncidentStatus(string id, [FromBody] UpdateIncidentStatusRequest request)
        {
            var success = await _incidentService.UpdateIncidentStatusAsync(id, request.Status);
            if (!success) return NotFound();
            return Ok(new { message = "Status updated successfully" });
        }
    }
}
