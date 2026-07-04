using System;
using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewDto> CreateReviewAsync(string userId, CreateReviewDto dto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == dto.BookingId && b.CustomerId == userId);

            if (booking == null)
                throw new Exception("Booking not found or you don't have permission.");

            if (booking.Status == "Cancelled")
                throw new Exception("Cannot review a cancelled booking.");

            var existingReview = await _context.ServiceReviews
                .FirstOrDefaultAsync(r => r.BookingId == dto.BookingId);

            if (existingReview != null)
                throw new Exception("Review already exists for this booking.");

            var review = new ServiceReview
            {
                Id = $"{dto.BookingId}-{DateTime.UtcNow:ddMMyy}",
                BookingId = dto.BookingId,
                CustomerId = userId,
                BranchId = booking.BranchId,
                OverallRating = dto.ServiceRating,
                CleanlinessRating = dto.CleanlinessRating,
                SpeedRating = dto.SpeedRating,
                StaffRating = dto.StaffRating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.ServiceReviews.Add(review);
            await _context.SaveChangesAsync();

            return MapToDto(review);
        }

        public async Task<ReviewDto?> GetReviewByBookingIdAsync(string bookingId)
        {
            var review = await _context.ServiceReviews
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);

            return review != null ? MapToDto(review) : null;
        }

        public async Task<ReviewDto> UpdateReviewAsync(string userId, string bookingId, UpdateReviewDto dto)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.CustomerId == userId);

            if (booking == null)
                throw new Exception("Booking not found or you don't have permission.");

            var review = await _context.ServiceReviews
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);

            if (review == null)
                throw new Exception("Review not found.");

            review.OverallRating = dto.ServiceRating;
            review.CleanlinessRating = dto.CleanlinessRating;
            review.SpeedRating = dto.SpeedRating;
            review.StaffRating = dto.StaffRating;
            review.Comment = dto.Comment;

            await _context.SaveChangesAsync();

            return MapToDto(review);
        }

        public async Task<bool> DeleteReviewAsync(string userId, string bookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.CustomerId == userId);

            if (booking == null)
                throw new Exception("Booking not found or you don't have permission.");

            var review = await _context.ServiceReviews
                .FirstOrDefaultAsync(r => r.BookingId == bookingId);

            if (review == null)
                throw new Exception("Review not found.");

            _context.ServiceReviews.Remove(review);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<System.Collections.Generic.IEnumerable<ReviewDto>> GetReviewsByBranchAsync(string branchId)
        {
            var reviews = await _context.ServiceReviews
                .Include(r => r.RespondedBy)
                .Include(r => r.Booking)
                .ThenInclude(b => b.Customer)
                .Where(r => r.BranchId == branchId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                BookingId = r.BookingId,
                BranchId = r.BranchId,
                OverallRating = r.OverallRating,
                CleanlinessRating = r.CleanlinessRating,
                SpeedRating = r.SpeedRating,
                StaffRating = r.StaffRating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                ResponseText = r.ResponseText,
                RespondedById = r.RespondedById,
                RespondedByName = r.RespondedBy?.FullName,
                RespondedAt = r.RespondedAt,
                CustomerName = r.Booking?.Customer?.FullName ?? "Khách hàng"
            });
        }

        public async Task<bool> RespondToReviewAsync(string reviewId, string respondedById, string responseText)
        {
            var review = await _context.ServiceReviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null) return false;

            review.ResponseText = responseText;
            review.RespondedById = respondedById;
            review.RespondedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private ReviewDto MapToDto(ServiceReview review)
        {
            var customerName = _context.Bookings.Include(b => b.Customer).FirstOrDefault(b => b.Id == review.BookingId)?.Customer?.FullName ?? "Khách hàng";
            var respondedByName = review.RespondedById != null ? _context.Users.FirstOrDefault(u => u.Id == review.RespondedById)?.FullName : null;

            return new ReviewDto
            {
                Id = review.Id,
                BookingId = review.BookingId,
                BranchId = review.BranchId,
                OverallRating = review.OverallRating,
                CleanlinessRating = review.CleanlinessRating,
                SpeedRating = review.SpeedRating,
                StaffRating = review.StaffRating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                ResponseText = review.ResponseText,
                RespondedById = review.RespondedById,
                RespondedByName = respondedByName,
                RespondedAt = review.RespondedAt,
                CustomerName = customerName
            };
        }
    }
}
