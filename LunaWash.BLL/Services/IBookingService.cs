using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDTO?> CreateBookingAsync(string userId, CreateBookingRequestDTO dto);
        Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<OccupiedSlotDTO>> GetOccupiedSlotsAsync(string date, string washSlotId);
        Task<bool> CancelBookingAsync(string userId, string bookingId);
        Task<bool> HardDeleteBookingAsync(string userId, string bookingId);

        Task<IEnumerable<BookingResponseDTO>> GetTodayBookingsForStaffAsync(string branchId, string? dateString = null);
        Task<bool> UpdateBookingStatusAsync(string bookingId, string newStatus);
        Task<(bool Success, string Message)> AddInteriorCleaningAsync(string bookingId);

        Task<IEnumerable<string>> GetAvailableTimeSlotsAsync(string branchId, DateOnly date);
    }
}
