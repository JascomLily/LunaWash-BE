using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IReviewService
    {
        Task<bool> SubmitReviewAsync(CreateReviewDto dto);
        Task<bool> DeleteReviewByBookingAsync(string bookingId);
    }
}