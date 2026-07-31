using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Services;

namespace LunaWash.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VouchersController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VouchersController(IVoucherService voucherService)
    {
        _voucherService = voucherService;
    }

    /// <summary>
    /// API xử lý chức năng: Lấy danh sách / thông tin active vouchers
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetActiveVouchers()
    {
        var vouchers = await _voucherService.GetActiveVouchersAsync();
        return Ok(new { success = true, data = vouchers });
    }

    [Authorize(Roles = "Admin")]
    /// <summary>
    /// API xử lý chức năng: Lấy danh sách / thông tin all vouchers
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllVouchers()
    {
        var vouchers = await _voucherService.GetAllVouchersAsync();
        return Ok(new { success = true, data = vouchers });
    }

    [Authorize(Roles = "Admin")]
    /// <summary>
    /// API xử lý chức năng: Tạo mới voucher
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherDto dto)
    {
        try
        {
            var voucher = await _voucherService.CreateVoucherAsync(dto);
            return Ok(new { success = true, data = voucher, message = "Tạo mã thành công!" });
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    /// <summary>
    /// API xử lý chức năng: Xóa voucher
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVoucher(string id)
    {
        var result = await _voucherService.DeleteVoucherAsync(id);
        if (!result) return NotFound(new { success = false, message = "Không tìm thấy voucher hoặc đã bị xóa." });
        
        return Ok(new { success = true, message = "Xóa voucher thành công." });
    }

    [Authorize]
    /// <summary>
    /// API xử lý chức năng: Lấy danh sách / thông tin my vouchers
    /// </summary>
    [HttpGet("my-vouchers")]
    public async Task<IActionResult> GetMyVouchers()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var vouchers = await _voucherService.GetMyVouchersAsync(userId);
        return Ok(new { success = true, data = vouchers });
    }

    [Authorize]
    /// <summary>
    /// API xử lý chức năng: Save voucher
    /// </summary>
    [HttpPost("save/{voucherId}")]
    public async Task<IActionResult> SaveVoucher(string voucherId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _voucherService.SaveVoucherAsync(userId, voucherId);
        if (!result) return BadRequest(new { success = false, message = "Không thể lưu voucher này. Voucher không hợp lệ hoặc bạn đã lưu rồi." });

        return Ok(new { success = true, message = "Đã lưu voucher vào ví." });
    }
}
