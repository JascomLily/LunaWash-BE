using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;

namespace LunaWash.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SubmitReviewAsync(CreateReviewDto dto)
        {
            // No ambiguous mapping allowed

            var review = new ServiceReview
            {
                Id = "REV-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                BookingId = dto.BookingId,
                OverallRating = dto.OverallRating,
                CleanlinessRating = dto.CleanlinessRating,
                SpeedRating = dto.SpeedRating,
                StaffRating = dto.StaffRating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.ServiceReviews.Add(review);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteReviewByBookingAsync(string bookingId)
        {
            var review = await _context.ServiceReviews.FirstOrDefaultAsync(r => r.BookingId == bookingId);
            if (review == null) return false;

            _context.ServiceReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}