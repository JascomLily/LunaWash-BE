using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LunaWash.API.BackgroundServices
{
    public class BookingCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingCleanupService> _logger;
        // Chạy dọn dẹp mỗi 5 phút
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public BookingCleanupService(IServiceProvider serviceProvider, ILogger<BookingCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BookingCleanupService started. It will run every 5 minutes.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupPendingBookingsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up pending bookings.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("BookingCleanupService is stopping.");
        }

        private async Task CleanupPendingBookingsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Thời hạn: Những booking Pending đã tồn tại quá 10 phút
            var thresholdTime = DateTime.UtcNow.AddMinutes(-10);

            // Lấy danh sách booking quá hạn
            var expiredBookings = await context.Bookings
                .Where(b => b.Status == "Pending" && b.CreatedAt < thresholdTime)
                .ToListAsync(cancellationToken);

            if (!expiredBookings.Any())
            {
                return;
            }

            _logger.LogInformation($"Found {expiredBookings.Count} expired 'Pending' bookings. Initiating Hard Delete...");

            foreach (var booking in expiredBookings)
            {
                // 1. Hoàn lại điểm (Loyalty Points) nếu người dùng đã dùng điểm để thanh toán VNPay
                var pointHistories = await context.PointHistories
                    .Where(p => p.BookingId == booking.Id)
                    .ToListAsync(cancellationToken);

                if (pointHistories.Any())
                {
                    // Lấy ra số điểm đã bị trừ (số âm)
                    var deductedPoints = pointHistories
                        .Where(p => p.Points < 0)
                        .Sum(p => p.Points);

                    if (deductedPoints < 0)
                    {
                        var refundAmount = Math.Abs(deductedPoints);
                        var customer = await context.CustomerProfiles
                            .FirstOrDefaultAsync(c => c.UserId == booking.CustomerId, cancellationToken);
                        
                        if (customer != null)
                        {
                            // Cộng lại điểm cho khách
                            customer.CurrentPoints += refundAmount;

                            // Ghi lại log hoàn điểm (không gán BookingId vì booking sẽ bị xóa)
                            var refundLog = new PointHistory
                            {
                                UserId = booking.CustomerId,
                                Points = refundAmount,
                                RemainingPoints = refundAmount,
                                Description = $"Hoàn lại điểm do giao dịch VNPay hết hạn (tự động xóa)",
                                CreatedAt = DateTime.UtcNow
                            };
                            context.PointHistories.Add(refundLog);
                        }
                    }

                    // Xóa các PointHistory rác liên quan đến Booking này để tránh lỗi khóa ngoại hoặc rác DB
                    context.PointHistories.RemoveRange(pointHistories);
                }

                // 2. Xóa Booking (Tối ưu Database)
                context.Bookings.Remove(booking);
            }

            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Successfully cleaned up {expiredBookings.Count} expired 'Pending' bookings.");
        }
    }
}
