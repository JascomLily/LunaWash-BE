using System;
using System.Collections.Generic;
using System.Data; // Cần thiết cho IsolationLevel
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
            var startTime = dto.ScheduledStartTime;
            if (startTime.Kind == DateTimeKind.Utc) 
            {
                startTime = startTime.AddHours(7);
            }
            
            int durationMinutes = dto.Duration > 0 ? dto.Duration : 30; // Lấy từ nhánh main
            var endTime = startTime.AddMinutes(durationMinutes);
            var bookingDate = startTime.Date;
  
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var allBranchSlots = await _context.WashSlots
                    .Where(ws => ws.BranchId == dto.BranchId && !ws.IsDeleted)
                    .ToListAsync();

                if (!allBranchSlots.Any())
                {
                    throw new InvalidOperationException("Chi nhánh này chưa được cấu hình cầu rửa (WashSlot).");
                }

                var busySlotIds = await _context.Bookings
                    .Where(b => b.BranchId == dto.BranchId
                             && b.Status != "Cancelled" 
                             && !b.IsDeleted
                             && b.ScheduledStartTime < endTime 
                             && b.ScheduledEndTime > startTime)
                    .Select(b => b.WashSlotId)
                    .ToListAsync();

                var availableSlot = allBranchSlots.FirstOrDefault(slot => !busySlotIds.Contains(slot.Id));

                if (availableSlot == null)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("Rất tiếc, khung giờ này đã kín lịch. Vui lòng chọn giờ khác.");
                }

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

                // Gộp khai báo biến từ 2 nhánh
                string packageName = "Gói Cơ Bản";
                string services = "Rửa sạch ngoại thất, làm khô tự động và xịt bóng lốp.";
                int basePrice = 150000;

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

                // Code xử lý slot lấy từ nhánh main
                var washSlot = !string.IsNullOrEmpty(dto.WashSlotId)
                    ? await _context.WashSlots.FirstOrDefaultAsync(ws => ws.Id == dto.WashSlotId && ws.BranchId == dto.BranchId)
                    : await _context.WashSlots.FirstOrDefaultAsync(ws => ws.BranchId == dto.BranchId);
                string dbSlotId = washSlot?.Id ?? availableSlot?.Id ?? "BRN-BT-01-WS-01"; 

                // Khởi tạo notesObj từ nhánh main
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
                    IsDeleted = false,
                    BookingDate = DateOnly.FromDateTime(bookingDate),
                    TotalPrice = totalPrice
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                // Nạp Reference từ nhánh main trước khi trả về
                await _context.Entry(booking).Reference(b => b.Branch).LoadAsync();

                return BuildBookingResponse(booking);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CompleteBookingAsync(string bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return false;

            booking.Status = "Completed"; // Hoặc trạng thái tương đương của dự án

            // ĐIỀU KIỆN TÍCH ĐIỂM: Ví dụ mỗi 10.000đ đơn hàng được tính là 1 điểm
            int earnedPoints = (int)(booking.TotalPrice / 10000);

            if (earnedPoints > 0)
            {
                var profile = await _context.CustomerProfiles.FirstOrDefaultAsync(cp => cp.UserId == booking.CustomerId);
                if (profile != null)
                {
                    // 1. Lưu lịch sử cộng điểm kèm thời hạn 1 năm
                    var pointLog = new PointHistory
                    {
                        UserId = profile.UserId,
                        Points = earnedPoints,
                        RemainingPoints = earnedPoints, // Ban đầu toàn bộ điểm này đều khả dụng
                        Description = $"Tích điểm từ đơn đặt lịch thành công #{booking.Id}",
                        BookingId = booking.Id,
                        CreatedAt = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddYears(1), // ĐIỀU KIỆN HẾT HẠN 1 NĂM
                        IsExpired = false
                    };
                    _context.PointHistories.Add(pointLog);

                    // 2. Cập nhật vào hồ sơ khách hàng
                    profile.CurrentPoints += earnedPoints;
                    profile.AccumulatedPoints += earnedPoints; // Điểm trọn đời để giữ/lên hạng
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Branch)
                .Where(b => b.CustomerId == userId)
                .OrderByDescending(b => b.ScheduledStartTime)
                .ToListAsync();

            var bookingIds = bookings.Select(b => b.Id).ToList();
            var reviews = await _context.ServiceReviews
                .Where(r => bookingIds.Contains(r.BookingId))
                .ToDictionaryAsync(r => r.BookingId, r => r.OverallRating);

            return bookings.Select(b => {
                var dto = BuildBookingResponse(b);
                if (reviews.TryGetValue(b.Id, out double rating))
                {
                    dto.Rating = rating;
                }
                return dto;
            });
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

        public async Task<IEnumerable<BookingResponseDTO>> GetTodayBookingsForStaffAsync(string branchId, string? dateString = null)
        {
            DateOnly targetDate;
            if (!string.IsNullOrEmpty(dateString) && DateOnly.TryParse(dateString, out var parsedDate))
            {
                targetDate = parsedDate;
            }
            else
            {
                targetDate = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
            }

            var bookings = await _context.Bookings
                .Where(b => b.BranchId == branchId 
                         && b.BookingDate == targetDate 
                         && !b.IsDeleted)
                .OrderBy(b => b.ScheduledStartTime)
                .ToListAsync();

            var bookingIds = bookings.Select(b => b.Id).ToList();
            var reviews = await _context.ServiceReviews
                .Where(r => bookingIds.Contains(r.BookingId))
                .ToDictionaryAsync(r => r.BookingId, r => r.OverallRating);

            return bookings.Select(b => {
                var dto = BuildBookingResponse(b);
                if (reviews.TryGetValue(b.Id, out double rating))
                {
                    dto.Rating = rating;
                }
                return dto;
            });
        }
        
        public async Task<bool> UpdateBookingStatusAsync(string bookingId, string newStatus)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);
            if (booking == null) return false;

            // Nếu đang là Completed mà lại truyền Completed tiếp thì bỏ qua (tránh cộng điểm 2 lần)
            if (booking.Status == "Completed" && newStatus == "Completed")
                return true;

            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.UtcNow;
            
            // Xử lý Check-in
            if ((newStatus == "Checked-In" || newStatus == "Washing") && booking.CheckInTime == null)
            {
                booking.CheckInTime = DateTime.UtcNow.AddHours(7);
            }

            // ==========================================
            // XỬ LÝ LOYALTY: CỘNG ĐIỂM KHI HOÀN THÀNH
            // ==========================================
            if (newStatus == "Completed")
            {
                if (booking.CheckoutTime == null)
                {
                    booking.CheckoutTime = DateTime.UtcNow.AddHours(7);
                }

                int totalPrice = 0;
                
                // Lấy tổng tiền từ cột Notes (Lưu JSON)
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
                    catch { /* Bỏ qua nếu lỗi parse JSON */ }
                }

                if (totalPrice > 0)
                {
                    // Tính điểm: 5% giá trị hóa đơn (VD: 150.000đ -> 7.500 điểm/VND)
                    int earnedPoints = (int)(totalPrice * 0.05);

                    // Lấy Profile của khách hàng
                    var customerProfile = await _context.CustomerProfiles
                        .FirstOrDefaultAsync(cp => cp.UserId == booking.CustomerId);

                    // Nếu khách chưa có Profile thì tạo mới
                    if (customerProfile == null)
                    {
                        // Tìm hạng thành viên mặc định (Thấp nhất - ví dụ Member)
                        var defaultTier = await _context.MembershipTiers
                            .OrderBy(t => t.MinPoints)
                            .FirstOrDefaultAsync() 
                            ?? new MembershipTier 
                            { 
                                Id = "TIER-01", 
                                TierName = "Member", 
                                MinPoints = 0, 
                                PointsMultiplier = 1,
                                DiscountPercent = 0
                            }; // Fallback nếu DB chưa có Tier

                        if (defaultTier.CreatedAt == default) 
                        {
                            _context.MembershipTiers.Add(defaultTier);
                        }

                        customerProfile = new CustomerProfile
                        {
                            UserId = booking.CustomerId,
                            CurrentPoints = 0,
                            AccumulatedPoints = 0,
                            MembershipTierId = defaultTier.Id
                        };
                        _context.CustomerProfiles.Add(customerProfile);
                    }

                    // Cộng điểm
                    customerProfile.CurrentPoints += earnedPoints;
                    customerProfile.AccumulatedPoints += earnedPoints;

                    // [Tùy chọn] Logic tự động nâng hạng:
                    // Lấy tất cả các hạng, tìm hạng cao nhất mà AccumulatedPoints đạt được
                    var eligibleTier = await _context.MembershipTiers
                        .Where(t => t.MinPoints <= customerProfile.AccumulatedPoints && !t.IsDeleted)
                        .OrderByDescending(t => t.MinPoints)
                        .FirstOrDefaultAsync();

                    if (eligibleTier != null && customerProfile.MembershipTierId != eligibleTier.Id)
                    {
                        // Nâng hạng cho khách!
                        customerProfile.MembershipTierId = eligibleTier.Id;
                    }
                }
            }
            
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
                Status = b.Status == "Cancelled" ? "Đã hủy" : 
                         b.Status == "Completed" ? "Hoàn thành" : 
                         (b.Status == "Washing" || b.Status == "Checked-In") ? "Đang rửa" : 
                         "Sắp đến",
                PaymentMethod = paymentMethod,
                BookingDate = b.BookingDate.ToDateTime(TimeOnly.MinValue),
                CheckoutTime = b.CheckoutTime
            };
        }

        public async Task<IEnumerable<string>> GetAvailableTimeSlotsAsync(string branchId, DateOnly date)
        {
            var availableSlots = new List<string>();
            
            // 1. Giả định Giờ làm việc của tiệm: 08:00 đến 18:00 (mỗi slot 30 phút)
            // Tương lai bạn có thể kéo từ bảng Branch Configuration nếu các chi nhánh có giờ làm việc khác nhau
            var openTime = new TimeSpan(8, 0, 0);
            var closeTime = new TimeSpan(18, 0, 0);
            var slotDuration = TimeSpan.FromMinutes(30);

            // 2. Lấy tổng số cầu rửa của chi nhánh (Tổng công suất phục vụ cùng lúc)
            var totalWashSlots = await _context.WashSlots
                .CountAsync(ws => ws.BranchId == branchId && !ws.IsDeleted);
                
            if (totalWashSlots == 0) return availableSlots; // Nếu chưa setup cầu rửa thì không có slot nào

            // 3. Lấy tất cả lịch đặt TRONG NGÀY đó của chi nhánh
            var bookingsOnDate = await _context.Bookings
                .Where(b => b.BranchId == branchId 
                         && b.BookingDate == date 
                         && b.Status != "Cancelled" 
                         && !b.IsDeleted)
                .ToListAsync();

            // 4. Mốc thời gian hiện tại để so sánh (không cho khách đặt giờ trong quá khứ)
            var currentTimeVn = DateTime.UtcNow.AddHours(7);
            var isToday = date == DateOnly.FromDateTime(currentTimeVn);

            // 5. Quét từng khung 30 phút trong ngày
            for (var time = openTime; time < closeTime; time += slotDuration)
            {
                var slotStart = date.ToDateTime(TimeOnly.FromTimeSpan(time));
                var slotEnd = slotStart.Add(slotDuration);

                // NẾU là ngày hôm nay: Bỏ qua các khung giờ trong quá khứ 
                // (Chặn thêm 30p buffer: Ví dụ bây giờ là 08:15 thì không cho đặt 08:30 nữa, chỉ cho đặt từ 09:00 trở đi)
                if (isToday && slotStart <= currentTimeVn.AddMinutes(30))
                {
                    continue;
                }

                // Đếm số lượng xe ĐANG RỬA trong khung giờ này
                // Điều kiện chồng lấn (Overlap): Lịch Start < SlotEnd VÀ Lịch End > SlotStart
                var overlappingBookings = bookingsOnDate.Count(b => 
                    b.ScheduledStartTime < slotEnd && b.ScheduledEndTime > slotStart);

                // Nếu số xe đang rửa < tổng số máy rửa => Vẫn còn chỗ (Available)
                if (overlappingBookings < totalWashSlots)
                {
                    availableSlots.Add(time.ToString(@"hh\:mm")); // Trả về dạng "08:00", "08:30"
                }
            }

            return availableSlots;
        }

        public async Task<bool> UsePointsAsync(string userId, int pointsToUse)
        {
            var profile = await _context.CustomerProfiles.FirstOrDefaultAsync(cp => cp.UserId == userId);
            if (profile == null || profile.CurrentPoints < pointsToUse) return false;

            // Lấy tất cả các đợt CỘNG điểm còn hạn sử dụng, xếp từ cũ nhất đến mới nhất (FIFO)
            var availablePoints = await _context.PointHistories
                .Where(p => p.UserId == userId && p.RemainingPoints > 0 && p.ExpiryDate > DateTime.UtcNow && !p.IsExpired)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            int pointsLeftToDeduct = pointsToUse;

            foreach (var record in availablePoints)
            {
                if (record.RemainingPoints >= pointsLeftToDeduct)
                {
                    record.RemainingPoints -= pointsLeftToDeduct;
                    pointsLeftToDeduct = 0;
                    break;
                }
                else
                {
                    pointsLeftToDeduct -= record.RemainingPoints;
                    record.RemainingPoints = 0; // Đợt điểm này đã bị tiêu hết sạch
                }
            }

            // Nếu đã khấu trừ đủ cấu trúc điểm hợp lệ
            if (pointsLeftToDeduct == 0)
            {
                // Ghi log giao dịch TRỪ điểm
                var deductLog = new PointHistory
                {
                    UserId = userId,
                    Points = -pointsToUse,
                    RemainingPoints = 0,
                    Description = "Sử dụng điểm thưởng để giảm giá đơn hàng",
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = null // Giao dịch trừ không cần ngày hết hạn
                };
                _context.PointHistories.Add(deductLog);

                // Cập nhật lại số điểm hiện tại của tài khoản
                profile.CurrentPoints -= pointsToUse;

                await _context.SaveChangesAsync();
                return true;
            }

            return false; // Không đủ điểm hợp lệ (hoặc điểm đã bị hết hạn trước đó)
        }

        public async Task<(bool Success, string Message)> AddInteriorCleaningAsync(string bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);
            if (booking == null) return (false, "Không tìm thấy lịch đặt.");
            if (booking.Status != "Pending") return (false, "Chỉ có thể thêm dịch vụ khi xe đang chờ.");

            // Xác định số lượng slot cần thêm dựa trên loại xe
            int totalSlots = 1;
            int extraPrice = 0;

            switch (booking.VehicleTypeId)
            {
                case "VT-OTO-2C": totalSlots = 3; extraPrice = 500000; break;
                case "VT-OTO-4C": totalSlots = 4; extraPrice = 700000; break;
                case "VT-OTO-7C": totalSlots = 5; extraPrice = 1000000; break;
                case "VT-OTO-BT": totalSlots = 6; extraPrice = 1100000; break;
                case "VT-OTO-SUV": totalSlots = 6; extraPrice = 1100000; break;
                default: totalSlots = 4; extraPrice = 700000; break;
            }

            int extraSlots = totalSlots - 1; 
            if (extraSlots <= 0) return (false, "Loại xe này không áp dụng thêm slot.");

            // Giả định mỗi slot kéo dài 30 phút, hoặc tính toán dựa trên tổng thời lượng.
            // Ở front-end, thời gian slot là 30 phút, booking 1 slot chiếm 30p (dù rửa có thể nhanh hơn).
            var newEndTime = booking.ScheduledEndTime.AddMinutes(extraSlots * 30);

            // Kiểm tra trùng lịch trên cùng trạm
            var overlappingCount = await _context.Bookings
                .Where(b => b.WashSlotId == booking.WashSlotId
                         && b.BookingDate == booking.BookingDate
                         && b.Status != "Cancelled"
                         && !b.IsDeleted
                         && b.Id != booking.Id
                         && b.ScheduledStartTime < newEndTime
                         && b.ScheduledEndTime > booking.ScheduledEndTime) // Chỉ check các xe nằm sau booking hiện tại
                .CountAsync();

            if (overlappingCount > 0)
            {
                return (false, $"Kẹt lịch! Không đủ {extraSlots} khung giờ (slot) trống liên tiếp ngay phía sau trên cùng trạm này.");
            }

            booking.ScheduledEndTime = newEndTime;
            booking.TotalPrice += extraPrice;

            if (!string.IsNullOrEmpty(booking.Notes))
            {
                try
                {
                    var node = System.Text.Json.Nodes.JsonNode.Parse(booking.Notes);
                    if (node != null)
                    {
                        var currentServices = node["services"]?.ToString();
                        if (!string.IsNullOrEmpty(currentServices) && !currentServices.Contains("Vệ sinh nội thất"))
                        {
                            node["services"] = currentServices + " + Vệ sinh nội thất";
                        }
                        
                        var currentPackage = node["packageName"]?.ToString();
                        if (!string.IsNullOrEmpty(currentPackage) && !currentPackage.Contains("VỆ SINH NỘI THẤT"))
                        {
                            node["packageName"] = currentPackage + " + VỆ SINH NỘI THẤT";
                        }

                        node["totalPrice"] = booking.TotalPrice;
                        node["timeRange"] = $"{booking.ScheduledStartTime:HH:mm} - {newEndTime:HH:mm}";
                        booking.Notes = node.ToJsonString();
                    }
                }
                catch { }
            }

            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (true, "Đã thêm dịch vụ vệ sinh nội thất thành công.");
        }
    }
}