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

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin all employees
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin employees by branch
        /// </summary>
        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetEmployeesByBranch(string branchId)
        {
            var employees = await _employeeService.GetEmployeesByBranchAsync(branchId);
            return Ok(employees);
        }

        /// <summary>
        /// API xử lý chức năng: Cập nhật salary
        /// </summary>
        [HttpPut("{id}/salary")]
        public async Task<IActionResult> UpdateSalary(string id, [FromBody] UpdateSalaryRequest request)
        {
            var result = await _employeeService.UpdateEmployeeSalaryAsync(id, request.Salary);
            if (!result) return NotFound("Không tìm thấy nhân viên hoặc hồ sơ nhân sự.");
            return Ok(new { message = "Cập nhật lương thành công" });
        }

        /// <summary>
        /// API xử lý chức năng: Cập nhật status
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _employeeService.UpdateEmployeeStatusAsync(id, request.IsActive);
            if (!result) return NotFound("Không tìm thấy nhân viên.");
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }

        /// <summary>
        /// API xử lý chức năng: Thêm employee
        /// </summary>
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

        /// <summary>
        /// API xử lý chức năng: Xóa employee
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Xóa nhân viên thành công" });
        }

        /// <summary>
        /// API xử lý chức năng: Kiểm tra in
        /// </summary>
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            var result = await _employeeService.CheckInAsync(request.EmployeeId, request.BranchId);
            if (!result) return BadRequest("Đã Check-in hôm nay rồi.");
            return Ok(new { message = "Check-in thành công" });
        }

        /// <summary>
        /// API xử lý chức năng: Kiểm tra out
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            var result = await _employeeService.CheckOutAsync(request.EmployeeId);
            if (!result) return BadRequest("Chưa Check-in hoặc Đã Check-out rồi.");
            return Ok(new { message = "Check-out thành công" });
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin attendances
        /// </summary>
        [HttpGet("branch/{branchId}/attendance")]
        public async Task<IActionResult> GetAttendances(string branchId, [FromQuery] string date)
        {
            var attendances = await _employeeService.GetAttendancesByBranchAndDateAsync(branchId, date);
            return Ok(attendances);
        }

        /// <summary>
        /// API xử lý chức năng: Lấy danh sách / thông tin weekly leaves
        /// </summary>
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
