using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces;

public interface IReviewService
{
    Task<ReviewResponseDto> AddReviewAsync(string customerId, CreateReviewDto dto);
    Task<IEnumerable<ReviewResponseDto>> GetReviewsByBranchAsync(string branchId);
    Task<ReviewResponseDto> GetReviewByBookingIdAsync(string bookingId);
    Task<ReviewResponseDto> UpdateReviewByBookingIdAsync(string customerId, string bookingId, CreateReviewDto dto);
    Task<bool> DeleteReviewByBookingIdAsync(string customerId, string bookingId);
}
