using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IStaffManagementService _staffService;

        public UsersController(IStaffManagementService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("branch/{branchId}")]
        [Authorize]
        public async Task<IActionResult> GetEmployeesByBranch(string branchId)
        {
            try
            {
                var employees = await _staffService.GetEmployeesByBranchAsync(branchId);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
