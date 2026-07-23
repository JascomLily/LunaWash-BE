using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var overview = await _dashboardService.GetOverviewAsync();
            return Ok(overview);
        }

        [HttpGet("branch/{branchId}/revenue")]
        public async Task<IActionResult> GetBranchRevenue(string branchId, [FromQuery] string period = "week", [FromQuery] System.DateTime? referenceDate = null)
        {
            var overview = await _dashboardService.GetBranchRevenueOverviewAsync(branchId, period, referenceDate);
            return Ok(overview);
        }
    }
}
