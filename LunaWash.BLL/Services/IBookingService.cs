using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDTO?> CreateBookingAsync(string userId, CreateBookingRequestDTO dto);
        Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId);
        Task<bool> CancelBookingAsync(string userId, string bookingId);

        Task<IEnumerable<BookingResponseDTO>> GetTodayBookingsForStaffAsync(string branchId);
        Task<bool> UpdateBookingStatusAsync(string bookingId, string newStatus);

        Task<IEnumerable<string>> GetAvailableTimeSlotsAsync(string branchId, DateOnly date);
    }
}
