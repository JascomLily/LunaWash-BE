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
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly LunaWash.BLL.Interfaces.INotificationService _notificationService;
        private readonly ISettingsService _settingsService;

        public PaymentsController(IConfiguration configuration, ApplicationDbContext context, LunaWash.BLL.Interfaces.INotificationService notificationService, ISettingsService settingsService)
        {
            _configuration = configuration;
            _context = context;
            _notificationService = notificationService;
            _settingsService = settingsService;
        }

        [Authorize]
        /// <summary>
        /// Create a VNPAY payment link for a specific booking
        /// </summary>
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

            var settings = await _settingsService.GetPaymentSettingsAsync();
            var tmnCode = !string.IsNullOrEmpty(settings?.VnpayTmnCode) ? settings.VnpayTmnCode : _configuration["VnPay:TmnCode"];
            var hashSecret = !string.IsNullOrEmpty(settings?.VnpayHashSecret) ? settings.VnpayHashSecret : _configuration["VnPay:HashSecret"];

            // 3. Config VNPAY
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode!);
            vnpay.AddRequestData("vnp_Amount", (totalPrice * 100).ToString()); // VNPAY yêu cầu nhân 100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {bookingId}");
            vnpay.AddRequestData("vnp_OrderType", "other"); // Loại hàng hóa
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]!);
            vnpay.AddRequestData("vnp_TxnRef", bookingId); // Mã tham chiếu (mã đơn hàng)

            var paymentUrl = vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"]!, hashSecret!);

            return Ok(new { url = paymentUrl });
        }

        // VNPAY SẼ REDIRECT VỀ ĐÂY SAU KHI KHÁCH THANH TOÁN
        [AllowAnonymous]
        /// <summary>
        /// Handle the return request from VNPAY after user pays
        /// </summary>
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

            var settings = await _settingsService.GetPaymentSettingsAsync();
            var hashSecret = !string.IsNullOrEmpty(settings?.VnpayHashSecret) ? settings.VnpayHashSecret : _configuration["VnPay:HashSecret"];

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, hashSecret!);
            var frontendUrl = _configuration["VnPay:FrontendUrl"] ?? "http://localhost:5173";

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            decimal bookingAmount = booking?.TotalPrice ?? 0;

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00")
                {
                    // Thanh toán thành công -> Cập nhật trạng thái Booking
                    if (booking != null)
                    {
                        booking.Status = "Confirmed";
                        if (!string.IsNullOrEmpty(booking.Notes))
                        {
                            try
                            {
                                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                                var notesDict = JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(booking.Notes, options);
                                if (notesDict != null)
                                {
                                    notesDict["paymentMethod"] = "vnpay"; // Cập nhật thành vnpay (đã thanh toán thành công)
                                    booking.Notes = JsonSerializer.Serialize(notesDict);
                                }
                            }
                            catch { }
                        }
                        await _context.SaveChangesAsync();

                        // Gửi thông báo thanh toán thành công
                        await _notificationService.CreateNotificationAsync(
                            booking.CustomerId,
                            "Thanh toán thành công",
                            $"Thanh toán thành công {bookingAmount:N0}đ qua VNPay cho đơn hàng {booking.Id}.",
                            "Payment"
                        );
                    }
                    
                    return Redirect($"{frontendUrl}/payment?status=success&bookingId={bookingId}&amount={bookingAmount}");
                }
                else
                {
                    return Redirect($"{frontendUrl}/payment?status=failed&bookingId={bookingId}&errorCode={vnp_ResponseCode}&amount={bookingAmount}");
                }
            }

            return Redirect($"{frontendUrl}/payment?status=failed&bookingId={bookingId}&errorCode=InvalidSignature&amount={bookingAmount}");
        }
    }
}