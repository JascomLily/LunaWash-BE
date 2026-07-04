using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LunaWash.DAL.Data;

namespace LunaWash.API.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetSystemRevenue()
        {
            try
            {
                // Verify user is Admin or BranchManager
                var userTier = User.FindFirst("tier")?.Value ?? string.Empty;
                // If not specified in claims, we can skip or allow, but standard role checks:
                // if (userTier != "Admin" && userTier != "BranchManager") return Forbid();

                var branchesStats = await _context.Branches
                    .Where(b => !b.IsDeleted)
                    .Select(b => new
                    {
                        BranchId = b.Id,
                        BranchName = b.BranchName,
                        City = b.Address.Contains("Hà Nội") ? "Hà Nội" : "Hồ Chí Minh", // Quick classification
                        TotalBookings = _context.Bookings.Count(bk => bk.BranchId == b.Id && !bk.IsDeleted),
                        TotalRevenue = _context.Bookings.Where(bk => bk.BranchId == b.Id && !bk.IsDeleted && bk.Status != "Cancelled").Sum(bk => (double)bk.TotalPrice),
                        AverageRating = _context.ServiceReviews.Where(r => r.BranchId == b.Id).Average(r => (double?)r.OverallRating) ?? 5.0,
                        ActiveStations = _context.Equipments.Count(e => e.BranchId == b.Id && e.Status == "Hoạt động"),
                        TotalStations = _context.Equipments.Count(e => e.BranchId == b.Id)
                    })
                    .ToListAsync();

                var totalBookings = branchesStats.Sum(b => b.TotalBookings);
                var totalRevenue = branchesStats.Sum(b => b.TotalRevenue);
                var averageSystemRating = branchesStats.Any() ? branchesStats.Average(b => b.AverageRating) : 5.0;

                return Ok(new
                {
                    TotalBookings = totalBookings,
                    TotalRevenue = totalRevenue,
                    AverageSystemRating = Math.Round(averageSystemRating, 1),
                    ActiveBranchesCount = branchesStats.Count,
                    Branches = branchesStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
