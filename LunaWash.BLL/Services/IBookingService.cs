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
    }
}
