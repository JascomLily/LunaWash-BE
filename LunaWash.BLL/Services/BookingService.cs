using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponseDTO?> CreateBookingAsync(string userId, CreateBookingRequestDTO dto)
        {
            var vehicle = await _context.CustomerVehicles
                .Include(v => v.VehicleType)
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId && v.CustomerId == userId);

            if (vehicle == null) return null;

            // Xử lý gói dịch vụ và giá
            string packageName = "Gói Cơ Bản";
            string services = "Rửa sạch ngoại thất, làm khô tự động và xịt bóng lốp.";
            int basePrice = 150000;
            int durationMinutes = 15;

            if (dto.ServicePackageId == "PK-NC")
            {
                packageName = "Gói Nâng Cao";
                services = "Dịch vụ cơ bản kết hợp vệ sinh gầm và tẩy ố lazang.";
                basePrice = 250000;
                durationMinutes = 20;
            }
            else if (dto.ServicePackageId == "PK-CC")
            {
                packageName = "Gói Cao Cấp";
                services = "Chăm sóc toàn diện với phủ Nano Ceramic bảo vệ sơn xe.";
                basePrice = 500000;
                durationMinutes = 30;
            }

            int interiorPrice = dto.IncludeInteriorClean ? 1000000 : 0;
            if (dto.IncludeInteriorClean) durationMinutes += 15;
            int totalPrice = basePrice + interiorPrice;

            // Tính thời gian dựa trên TimeSlotId (VD: T-0900 -> 09:00)
            int startHour = 8;
            int startMinute = 0;
            if (dto.TimeSlotId.StartsWith("T-") && dto.TimeSlotId.Length == 6)
            {
                int.TryParse(dto.TimeSlotId.Substring(2, 2), out startHour);
                int.TryParse(dto.TimeSlotId.Substring(4, 2), out startMinute);
            }

            var now = DateTime.UtcNow.AddHours(7); // Giờ VN
            var bookingDate = now.Date;
            // Nếu giờ đặt nhỏ hơn giờ hiện tại, chuyển sang ngày mai
            if (startHour < now.Hour || (startHour == now.Hour && startMinute <= now.Minute))
            {
                bookingDate = bookingDate.AddDays(1);
            }

            var startTime = bookingDate.AddHours(startHour).AddMinutes(startMinute);
            var endTime = startTime.AddMinutes(durationMinutes);

            // Lưu thông tin phụ vào Notes dưới dạng JSON để tái tạo UI dễ dàng
            var notesObj = new {
                packageName = packageName,
                services = services,
                totalPrice = totalPrice,
                extras = dto.IncludeInteriorClean ? "KÈM VỆ SINH NỘI THẤT" : null,
                paymentMethod = dto.PaymentMethod,
                timeRange = $"{startTime:HH:mm} - {endTime:HH:mm}",
                displayDate = bookingDate == now.Date ? "Hôm nay" : (bookingDate == now.Date.AddDays(1) ? "Ngày mai" : bookingDate.ToString("dd/MM/yyyy")),
                vehicleInfo = $"{vehicle.VehicleModel} • {vehicle.LicensePlate}"
            };

            // Map UI IDs to valid DB IDs
            string dbBranchId = dto.BranchId == "BR-LD" ? "BRN-BT-01" : "BRN-Q1-01";
            string dbSlotId = dto.WashSlotId == "SL-01" ? "BRN-Q1-01-WS-01" : 
                             (dto.WashSlotId == "SL-02" ? "BRN-Q1-01-WS-02" : "BRN-BT-01-WS-01");

            var booking = new Booking
            {
                Id = "BKG-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                CustomerId = userId,
                BranchId = dbBranchId,
                VehicleTypeId = vehicle.VehicleTypeId ?? "VT-OTO-01",
                ScheduledStartTime = startTime,
                ScheduledEndTime = endTime,
                Status = "Confirmed",
                WashSlotId = dbSlotId,
                Notes = JsonSerializer.Serialize(notesObj),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            booking.BookingDate = DateOnly.FromDateTime(bookingDate);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return BuildBookingResponse(booking);
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.CustomerId == userId)
                .OrderByDescending(b => b.ScheduledStartTime)
                .ToListAsync();

            return bookings.Select(BuildBookingResponse);
        }

        public async Task<bool> CancelBookingAsync(string userId, string bookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.CustomerId == userId);

            if (booking == null) return false;

            booking.Status = "Cancelled";
            booking.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private BookingResponseDTO BuildBookingResponse(Booking b)
        {
            string packageName = "Gói Cơ Bản";
            string services = "";
            string extras = "";
            string totalPrice = "0đ";
            string paymentMethod = "tien-mat";
            string timeRange = $"{b.ScheduledStartTime:HH:mm} - {b.ScheduledEndTime:HH:mm}";
            string vehicleInfo = "";

            if (!string.IsNullOrEmpty(b.Notes))
            {
                try
                {
                    using var doc = JsonDocument.Parse(b.Notes);
                    if (doc.RootElement.TryGetProperty("packageName", out var pName)) packageName = pName.GetString() ?? packageName;
                    if (doc.RootElement.TryGetProperty("services", out var srv)) services = srv.GetString() ?? "";
                    if (doc.RootElement.TryGetProperty("extras", out var ext) && ext.ValueKind != JsonValueKind.Null) extras = ext.GetString() ?? "";
                    if (doc.RootElement.TryGetProperty("totalPrice", out var price)) totalPrice = string.Format(new System.Globalization.CultureInfo("vi-VN"), "{0:C0}", price.GetInt32());
                    if (doc.RootElement.TryGetProperty("paymentMethod", out var pm)) paymentMethod = pm.GetString() ?? paymentMethod;
                    if (doc.RootElement.TryGetProperty("vehicleInfo", out var vi)) vehicleInfo = vi.GetString() ?? "";
                }
                catch { }
            }

            return new BookingResponseDTO
            {
                Id = b.Id,
                PackageName = packageName,
                Services = services,
                VehicleInfo = vehicleInfo,
                Extras = extras,
                BranchInfo = b.BranchId == "BRN-Q1-01" ? "Chi nhánh Quận 1" : (b.BranchId == "BRN-BT-01" ? "Chi nhánh Bình Thạnh" : b.BranchId),
                SlotName = b.WashSlotId == "BRN-Q1-01-WS-01" ? "Trạm 1" : (b.WashSlotId == "BRN-Q1-01-WS-02" ? "Trạm 2" : "Trạm 1"),
                TimeRange = $"{timeRange}\n{b.ScheduledStartTime:dd/MM/yyyy}",
                TotalPrice = totalPrice,
                Status = b.Status == "Cancelled" ? "Đã hủy" : (b.ScheduledEndTime < DateTime.UtcNow.AddHours(7) ? "Hoàn thành" : "Sắp đến"),
                PaymentMethod = paymentMethod,
                BookingDate = b.BookingDate.ToDateTime(TimeOnly.MinValue)
            };
        }
    }
}
