using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using LunaWash.DAL.Data;
using LunaWash.BLL.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public PaymentsController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [Authorize]
        [HttpPost("create-vnpay-url/{bookingId}")]
        public async Task<IActionResult> CreateVnPayUrl(string bookingId)
        {
            // 1. Tìm đơn hàng
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);
            if (booking == null) return NotFound(new { message = "Không tìm thấy lịch đặt." });

            // 2. Lấy giá tiền từ JSON Notes
            int totalPrice = 0;
            if (!string.IsNullOrEmpty(booking.Notes))
            {
                try
                {
                    using var doc = JsonDocument.Parse(booking.Notes);
                    if (doc.RootElement.TryGetProperty("totalPrice", out var priceElement))
                    {
                        totalPrice = priceElement.GetInt32();
                    }
                }
                catch { }
            }

            if (totalPrice <= 0) return BadRequest("Lỗi: Số tiền thanh toán không hợp lệ.");

            // 3. Config VNPAY
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]!);
            vnpay.AddRequestData("vnp_Amount", (totalPrice * 100).ToString()); // VNPAY yêu cầu nhân 100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {bookingId}");
            vnpay.AddRequestData("vnp_OrderType", "other"); // Loại hàng hóa
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]!);
            vnpay.AddRequestData("vnp_TxnRef", bookingId); // Mã tham chiếu (mã đơn hàng)

            var paymentUrl = vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"]!, _configuration["VnPay:HashSecret"]!);

            return Ok(new { url = paymentUrl });
        }

        // VNPAY SẼ REDIRECT VỀ ĐÂY SAU KHI KHÁCH THANH TOÁN
        [AllowAnonymous]
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var vnpayData = Request.Query;
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            string bookingId = vnpay.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_SecureHash = Request.Query["vnp_SecureHash"].ToString();

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]!);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00")
                {
                    // Thanh toán thành công -> Cập nhật trạng thái Booking
                    var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
                    if (booking != null)
                    {
                        // Tuỳ logic dự án: Có thể update JSON "paymentMethod" sang "Đã thanh toán qua VNPAY"
                        // Hoặc cập nhật Status nếu bạn có status "Paid"
                        // booking.Status = "Paid";
                        await _context.SaveChangesAsync();
                    }
                    
                    return Ok(new { message = "Thanh toán thành công!", bookingId = bookingId });
                }
                else
                {
                    return BadRequest(new { message = $"Thanh toán thất bại. Mã lỗi: {vnp_ResponseCode}" });
                }
            }

            return BadRequest(new { message = "Sai chữ ký bảo mật (Invalid Signature)." });
        }
    }
}