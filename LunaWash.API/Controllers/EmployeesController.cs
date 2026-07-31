using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;
using LunaWash.BLL.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // [API: Nhận yêu cầu GET từ FE để lấy danh sách tất cả nhân viên]
        //<<Comment Function>>
        // Hàm này là: Trả về danh sách thông tin toàn bộ nhân viên trong hệ thống.
        //<</.....>>
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // [API: Nhận yêu cầu GET từ FE để lấy danh sách nhân viên theo chi nhánh]
        //<<Comment Function>>
        // Hàm này là: Lấy thông tin danh sách nhân viên thuộc về một chi nhánh cụ thể.
        //<</.....>>
        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetEmployeesByBranch(string branchId)
        {
            var employees = await _employeeService.GetEmployeesByBranchAsync(branchId);
            return Ok(employees);
        }

        // [API: Nhận yêu cầu PUT từ FE để cập nhật lương cho nhân viên]
        //<<Comment Function>>
        // Hàm này là: Thay đổi và cập nhật mức lương hiện tại của nhân viên dựa theo ID.
        //<</.....>>
        [HttpPut("{id}/salary")]
        public async Task<IActionResult> UpdateSalary(string id, [FromBody] UpdateSalaryRequest request)
        {
            var result = await _employeeService.UpdateEmployeeSalaryAsync(id, request.Salary);
            if (!result) return NotFound("Không tìm thấy nhân viên hoặc hồ sơ nhân sự.");
            return Ok(new { message = "Cập nhật lương thành công" });
        }

        // [API: Nhận yêu cầu PUT từ FE để cập nhật trạng thái làm việc của nhân viên]
        //<<Comment Function>>
        // Hàm này là: Đổi trạng thái hoạt động (kích hoạt/hủy kích hoạt) của một nhân viên.
        //<</.....>>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _employeeService.UpdateEmployeeStatusAsync(id, request.IsActive);
            if (!result) return NotFound("Không tìm thấy nhân viên.");
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }

        // [API: Nhận yêu cầu POST từ FE để thêm nhân viên mới vào hệ thống]
        //<<Comment Function>>
        // Hàm này là: Tạo một tài khoản nhân viên mới và lưu thông tin vào cơ sở dữ liệu.
        //<</.....>>
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeCreateDto dto)
        {
            try
            {
                var employee = await _employeeService.AddEmployeeAsync(dto);
                if (employee == null) return BadRequest("Không thể thêm nhân viên (Sai RoleId)");
                return Ok(employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // [API: Nhận yêu cầu DELETE từ FE để xóa một nhân viên]
        //<<Comment Function>>
        // Hàm này là: Xóa dữ liệu của một nhân viên khỏi hệ thống bằng ID.
        //<</.....>>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Xóa nhân viên thành công" });
        }

        // [API: Nhận yêu cầu POST từ FE để chấm công vào ca làm việc]
        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng check-in, đánh dấu nhân viên đã bắt đầu làm việc.
        //<</.....>>
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            var result = await _employeeService.CheckInAsync(request.EmployeeId, request.BranchId);
            if (!result) return BadRequest("Đã Check-in hôm nay rồi.");
            return Ok(new { message = "Check-in thành công" });
        }

        // [API: Nhận yêu cầu POST từ FE để chấm công kết thúc ca làm việc]
        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng check-out, đánh dấu nhân viên kết thúc giờ làm việc.
        //<</.....>>
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            var result = await _employeeService.CheckOutAsync(request.EmployeeId);
            if (!result) return BadRequest("Chưa Check-in hoặc Đã Check-out rồi.");
            return Ok(new { message = "Check-out thành công" });
        }

        // [API: Nhận yêu cầu GET từ FE để lấy danh sách điểm danh theo chi nhánh và ngày]
        //<<Comment Function>>
        // Hàm này là: Lấy dữ liệu điểm danh của tất cả nhân viên trong một chi nhánh vào ngày được chỉ định.
        //<</.....>>
        [HttpGet("branch/{branchId}/attendance")]
        public async Task<IActionResult> GetAttendances(string branchId, [FromQuery] string date)
        {
            var attendances = await _employeeService.GetAttendancesByBranchAndDateAsync(branchId, date);
            return Ok(attendances);
        }

        // [API: Nhận yêu cầu GET từ FE để lấy lịch nghỉ hàng tuần theo chi nhánh]
        //<<Comment Function>>
        // Hàm này là: Truy xuất thông tin các ngày nghỉ phép trong tuần của nhân viên thuộc chi nhánh.
        //<</.....>>
        [HttpGet("branch/{branchId}/weekly-leaves")]
        public async Task<IActionResult> GetWeeklyLeaves(string branchId, [FromQuery] string date)
        {
            var leaves = await _employeeService.GetWeeklyLeavesByBranchAsync(branchId, date);
            return Ok(leaves);
        }
    }

    public class CheckInRequest
    {
        public string EmployeeId { get; set; } = null!;
        public string BranchId { get; set; } = null!;
    }

    public class CheckOutRequest
    {
        public string EmployeeId { get; set; } = null!;
    }

    public class UpdateSalaryRequest
    {
        public decimal Salary { get; set; }
    }

    public class UpdateStatusRequest
    {
        public bool IsActive { get; set; }
    }
}
