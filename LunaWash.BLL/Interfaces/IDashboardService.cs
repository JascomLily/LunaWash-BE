using System.Collections.Generic;
using System.Threading.Tasks;

namespace LunaWash.BLL.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardOverviewDto> GetOverviewAsync();
        Task<BranchRevenueOverviewDto> GetBranchRevenueOverviewAsync(string branchId, string period, System.DateTime? referenceDate);
    }

    public class DashboardOverviewDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public List<BranchRevenueDto> RevenueByBranch { get; set; } = new List<BranchRevenueDto>();
        public List<RecentBookingDto> RecentBookings { get; set; } = new List<RecentBookingDto>();
    }

    public class BranchRevenueDto
    {
        public string BranchId { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public decimal Revenue { get; set; }
        public string Address { get; set; } = null!;
        public string Status { get; set; } = null!;
        public bool IsActive { get; set; }
        public string ActiveStations { get; set; } = null!;
        public double Rating { get; set; }
        public List<DailyRevenueDto> SparklineData { get; set; } = new List<DailyRevenueDto>();
    }

    public class DailyRevenueDto
    {
        public decimal Value { get; set; }
    }

    public class RecentBookingDto
    {
        public string Id { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Status { get; set; } = null!;
        public string Date { get; set; } = null!;
    }

    public class BranchRevenueOverviewDto
    {
        public decimal TodayRevenue { get; set; }
        public decimal ThisWeekRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public string CurrentPeriodLabel { get; set; } = null!;
        public List<RevenueDataPointDto> ChartData { get; set; } = new List<RevenueDataPointDto>();
        public List<RevenueDetailDto> Details { get; set; } = new List<RevenueDetailDto>();
    }

    public class RevenueDataPointDto
    {
        public string Label { get; set; } = null!; // E.g., "Thứ 2" or "Tuần 1"
        public decimal Revenue { get; set; }
        public int BookingsCount { get; set; }
        public string FullDate { get; set; } = null!;
    }

    public class RevenueDetailDto
    {
        public string Date { get; set; } = null!;
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
