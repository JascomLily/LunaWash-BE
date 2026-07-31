using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    // // Đây là code gì < Controller xử lý đường dẫn /api/Dashboard > <Quan trọng>
    // // Chức năng để làm gì < Cung cấp các API liên quan đến thống kê, báo cáo doanh thu >
    // // Nối vô FE ở đâu < Nối với trang AdminDashboard.jsx và ManagerRevenue.jsx trên Web FE >
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // // Đây là code gì < API lấy tổng quan doanh thu toàn hệ thống > <Bình thường>
        // // Chức năng để làm gì < Truy vấn DB lấy tổng số đơn, tổng tiền để vẽ biểu đồ cho Admin >
        // // Nối vô FE ở đâu < Được gọi trong file AdminDashboard.jsx của FE >
        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin overview
        /// </summary>
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var overview = await _dashboardService.GetOverviewAsync();
            return Ok(overview);
        }

        // // Đây là code gì < API lấy báo cáo doanh thu của 1 Chi nhánh cụ thể > <Rất quan trọng>
        // // Chức năng để làm gì < Lấy dữ liệu doanh thu theo tuần/tháng của 1 chi nhánh (Branch) >
        // // Nối vô FE ở đâu < Được gọi bằng hàm fetch() ở dòng 56 trong file ManagerRevenue.jsx của FE >
        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin branch revenue
        /// </summary>
        [HttpGet("branch/{branchId}/revenue")]
        public async Task<IActionResult> GetBranchRevenue(string branchId, [FromQuery] string period = "week", [FromQuery] System.DateTime? referenceDate = null)
        {
            // Gọi xuống tầng Service (BLL) để tính toán số liệu từ DB
            var overview = await _dashboardService.GetBranchRevenueOverviewAsync(branchId, period, referenceDate);
            // Trả kết quả (Cục JSON) về cho Frontend hiển thị
            return Ok(overview);
        }
    }
}
