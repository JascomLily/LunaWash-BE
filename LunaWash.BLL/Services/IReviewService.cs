using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(string userId, CreateReviewDto dto);
        Task<ReviewDto?> GetReviewByBookingIdAsync(string bookingId);
        Task<ReviewDto> UpdateReviewAsync(string userId, string bookingId, UpdateReviewDto dto);
        Task<bool> DeleteReviewAsync(string userId, string bookingId);
    }
}
