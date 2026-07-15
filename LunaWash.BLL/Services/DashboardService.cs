using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.BLL.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            var overview = new DashboardOverviewDto();

            // Total Bookings & Revenue
            var completedBookings = await _context.Bookings
                .Where(b => b.Status == "Completed" || b.Status == "Paid")
                .ToListAsync();

            overview.TotalBookings = completedBookings.Count;
            overview.TotalRevenue = completedBookings.Sum(b => b.TotalPrice);

            // Customers & Employees
            var customersCount = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Customer" && !u.IsDeleted)
                .CountAsync();
            overview.TotalCustomers = customersCount;

            var employeesCount = await _context.Users
                .Include(u => u.Role)
                .Where(u => (u.Role.RoleName == "Staff" || u.Role.RoleName == "TechnicalStaff") && !u.IsDeleted && u.IsActive)
                .CountAsync();
            overview.TotalEmployees = employeesCount;

            // Fetch auxiliary data for branch stats
            var allSlots = await _context.WashSlots.ToListAsync();
            var allReviews = await _context.ServiceReviews.ToListAsync();
            var today = DateTime.UtcNow.Date;
            var sevenDaysAgo = today.AddDays(-6);

            // Revenue by Branch
            var branches = await _context.Branches.Where(b => !b.IsDeleted).ToListAsync();
            foreach (var branch in branches)
            {
                var branchBookings = completedBookings
                    .Where(b => b.BranchId == branch.Id).ToList();

                // Lấy ngày giao dịch gần nhất của chi nhánh này làm mốc (để biểu đồ không bị phẳng lỳ nếu data cũ)
                var latestBooking = branchBookings.OrderByDescending(b => b.CreatedAt).FirstOrDefault();
                var referenceDate = latestBooking != null ? latestBooking.CreatedAt.Date : DateTime.UtcNow.Date;
                var chartStartDate = referenceDate.AddDays(-6);

                // Doanh thu của ngày có giao dịch gần nhất (DOANH THU NGÀY)
                var branchRevenue = branchBookings
                    .Where(b => b.CreatedAt.Date == referenceDate)
                    .Sum(b => b.TotalPrice);

                // Calculate Stations
                var branchSlots = allSlots.Where(s => s.BranchId == branch.Id).ToList();
                int totalSlots = branchSlots.Count;
                int activeSlots = branchSlots.Count(s => s.Status == "Available" || s.Status == "Active" || s.Status == "Occupied");
                
                bool isActive = activeSlots > 0 || totalSlots == 0; 
                string statusText = isActive ? "ĐANG HOẠT ĐỘNG" : "BẢO TRÌ TRẠM";
                string activeStationsText = $"{activeSlots}/{Math.Max(1, totalSlots)} trạm";

                // Calculate Rating
                var branchReviews = allReviews.Where(r => r.BranchId == branch.Id).ToList();
                double rating = branchReviews.Any() ? Math.Round(branchReviews.Average(r => r.OverallRating), 1) : 5.0;

                // Sparkline (Last 7 days of activity)
                var sparkline = new List<DailyRevenueDto>();
                for (int i = 0; i <= 6; i++)
                {
                    var date = chartStartDate.AddDays(i);
                    var dayRevenue = branchBookings
                        .Where(b => b.CreatedAt.Date == date)
                        .Sum(b => b.TotalPrice);
                    
                    sparkline.Add(new DailyRevenueDto { Value = dayRevenue });
                }

                overview.RevenueByBranch.Add(new BranchRevenueDto
                {
                    BranchId = branch.Id,
                    BranchName = branch.BranchName,
                    Revenue = branchRevenue,
                    Address = branch.Address ?? "Thủ Đức, HCM",
                    Status = statusText,
                    IsActive = isActive,
                    ActiveStations = activeStationsText,
                    Rating = rating,
                    SparklineData = sparkline
                });
            }

            // Recent Bookings
            var recent = await _context.Bookings
                .Include(b => b.Customer)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();

            foreach (var b in recent)
            {
                overview.RecentBookings.Add(new RecentBookingDto
                {
                    Id = b.Id,
                    CustomerName = b.Customer?.FullName ?? "Unknown",
                    ServiceName = "Dịch vụ rửa xe", // Mocking service name as it might require joining with BookingServices
                    Amount = b.TotalPrice,
                    Status = b.Status,
                    Date = b.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                });
            }

            return overview;
        }
    }
}
