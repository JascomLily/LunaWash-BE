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

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetEmployeesByBranch(string branchId)
        {
            var employees = await _employeeService.GetEmployeesByBranchAsync(branchId);
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeCreateDto dto)
        {
            var employee = await _employeeService.AddEmployeeAsync(dto);
            if (employee == null) return BadRequest("KhA'ng th thAm nhAn viAn (Sai RoleId)");
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "XA3a nhAn viAn thAnh cA'ng" });
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            var result = await _employeeService.CheckInAsync(request.EmployeeId, request.BranchId);
            if (!result) return BadRequest("?A? Check-in hA'm nay r?i.");
            return Ok(new { message = "Check-in thAnh cA'ng" });
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            var result = await _employeeService.CheckOutAsync(request.EmployeeId);
            if (!result) return BadRequest("Cha Check-in ho-c `A? Check-out r?i.");
            return Ok(new { message = "Check-out thAnh cA'ng" });
        }

        [HttpGet("attendance/branch/{branchId}")]
        public async Task<IActionResult> GetAttendances(string branchId, [FromQuery] string date)
        {
            var attendances = await _employeeService.GetAttendancesByBranchAndDateAsync(branchId, date);
            return Ok(attendances);
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
}
