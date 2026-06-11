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
            CustomerVehicle? vehicle = null;
            if (!string.IsNullOrEmpty(dto.LicensePlate))
            {
                vehicle = await _context.CustomerVehicles
                    .Include(v => v.VehicleType)
                    .FirstOrDefaultAsync(v => v.CustomerId == userId && v.LicensePlate == dto.LicensePlate);

                if (vehicle == null)
                {
                    vehicle = new CustomerVehicle
                    {
                        Id = "VEH-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                        CustomerId = userId,
                        VehicleTypeId = dto.VehicleTypeId,
                        LicensePlate = dto.LicensePlate,
                        VehicleModel = (dto.VehicleBrand + " " + dto.VehicleModel).Trim()
                    };
                    _context.CustomerVehicles.Add(vehicle);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                vehicle = await _context.CustomerVehicles
                    .Include(v => v.VehicleType)
                    .FirstOrDefaultAsync(v => v.CustomerId == userId && v.VehicleTypeId == dto.VehicleTypeId);

                if (vehicle == null)
                {
                    vehicle = await _context.CustomerVehicles
                        .Include(v => v.VehicleType)
                        .FirstOrDefaultAsync(v => v.CustomerId == userId);
                }
            }

            string packageName = "Gói Cơ Bản";
            string services = "Rửa sạch ngoại thất, làm khô tự động và xịt bóng lốp.";
            int basePrice = 150000;
            int durationMinutes = dto.Duration > 0 ? dto.Duration : 30;

            if (dto.ServicePriceIds != null && dto.ServicePriceIds.Any())
            {
                var servicePrices = await _context.ServicePrices
                    .Include(sp => sp.Service)
                    .Where(sp => dto.ServicePriceIds.Contains(sp.Id))
                    .ToListAsync();

                if (servicePrices.Any())
                {
                    basePrice = (int)servicePrices.Sum(sp => sp.Price);
                    services = string.Join(", ", servicePrices.Select(sp => sp.Service?.ServiceName ?? "Dịch vụ"));
                    if (dto.ServicePriceIds.Any(id => id.Contains("BSC"))) packageName = "Gói Cơ Bản";
                    else if (dto.ServicePriceIds.Any(id => id.Contains("ADV"))) packageName = "Gói Nâng Cao";
                    else if (dto.ServicePriceIds.Any(id => id.Contains("PRE"))) packageName = "Gói Cao Cấp";
                    else packageName = "Gói Tùy Chọn";
                }
            }

            string paymentMethod = dto.Notes != null && dto.Notes.Contains("VNPay") ? "vnpay" : "tien-mat";
            int totalPrice = basePrice;
            
            var startTime = dto.ScheduledStartTime;
            if (startTime.Kind == DateTimeKind.Utc) 
            {
                startTime = startTime.AddHours(7);
            }
            
            var endTime = startTime.AddMinutes(durationMinutes);
            var bookingDate = startTime.Date;

            var washSlot = !string.IsNullOrEmpty(dto.WashSlotId)
                ? await _context.WashSlots.FirstOrDefaultAsync(ws => ws.Id == dto.WashSlotId && ws.BranchId == dto.BranchId)
                : await _context.WashSlots.FirstOrDefaultAsync(ws => ws.BranchId == dto.BranchId);
            string dbSlotId = washSlot?.Id ?? "BRN-BT-01-WS-01"; 

            var notesObj = new {
                packageName = packageName,
                services = services,
                totalPrice = totalPrice,
                extras = dto.Notes,
                paymentMethod = paymentMethod,
                timeRange = $"{startTime:HH:mm} - {endTime:HH:mm}",
                displayDate = bookingDate.ToString("dd/MM/yyyy"),
                vehicleInfo = vehicle != null ? $"{vehicle.VehicleModel} • {vehicle.LicensePlate}" : "Xe khách hàng"
            };

            var booking = new Booking
            {
                Id = "BKG-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                CustomerId = userId,
                BranchId = dto.BranchId,
                VehicleTypeId = vehicle?.VehicleTypeId ?? dto.VehicleTypeId,
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
            await _context.Entry(booking).Reference(b => b.Branch).LoadAsync();

            return BuildBookingResponse(booking);
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Branch)
                .Where(b => b.CustomerId == userId)
                .OrderByDescending(b => b.ScheduledStartTime)
                .ToListAsync();

            return bookings.Select(BuildBookingResponse);
        }

        public async Task<IEnumerable<OccupiedSlotDTO>> GetOccupiedSlotsAsync(string date, string washSlotId)
        {
            if (!DateOnly.TryParse(date, out var bookingDate)) return new List<OccupiedSlotDTO>();

            var bookings = await _context.Bookings
                .Where(b => b.BookingDate == bookingDate && b.WashSlotId == washSlotId && b.Status != "Cancelled" && b.IsDeleted == false)
                .Select(b => new OccupiedSlotDTO
                {
                    StartTime = b.ScheduledStartTime,
                    EndTime = b.ScheduledEndTime
                })
                .ToListAsync();

            return bookings;
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
                BranchInfo = b.Branch?.BranchName ?? b.BranchId,
                SlotName = b.WashSlotId != null && b.WashSlotId.Contains("-WS-") ? "Trạm " + int.Parse(b.WashSlotId.Split('-').Last()) : "Trạm 1",
                TimeRange = $"{timeRange}\n{b.ScheduledStartTime:dd/MM/yyyy}",
                TotalPrice = totalPrice,
                Status = b.Status == "Cancelled" ? "Đã hủy" : (b.ScheduledEndTime < DateTime.UtcNow.AddHours(7) ? "Hoàn thành" : "Sắp đến"),
                PaymentMethod = paymentMethod,
                BookingDate = b.BookingDate.ToDateTime(TimeOnly.MinValue)
            };
        }
    }
}
