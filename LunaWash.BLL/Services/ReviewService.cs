using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace LunaWash.BLL.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewResponseDto> AddReviewAsync(string customerId, CreateReviewDto dto)
    {
        var booking = await _context.Bookings.FindAsync(dto.BookingId);
        if (booking == null || booking.CustomerId != customerId)
        {
            throw new Exception("Booking not found or not owned by user.");
        }

        if (booking.Status != "Completed")
        {
            throw new Exception("You can only review completed bookings.");
        }

        var existingReview = await _context.ServiceReviews.FirstOrDefaultAsync(r => r.BookingId == dto.BookingId);
        if (existingReview != null)
        {
            throw new Exception("Review already exists for this booking.");
        }

        var review = new ServiceReview
        {
            Id = $"{dto.BookingId}-{DateTime.Now:ddMMyy}",
            BookingId = dto.BookingId,
            CustomerId = customerId,
            BranchId = booking.BranchId,
            OverallRating = dto.OverallRating,
            CleanlinessRating = dto.CleanlinessRating,
            SpeedRating = dto.SpeedRating,
            StaffRating = dto.StaffRating,
            Comment = dto.Comment
        };

        _context.ServiceReviews.Add(review);
        await _context.SaveChangesAsync();

        return await MapToDto(review.Id);
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetReviewsByBranchAsync(string branchId)
    {
        var reviews = await _context.ServiceReviews
            .Include(r => r.Customer)
            .Where(r => r.BranchId == branchId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(r => new ReviewResponseDto
        {
            Id = r.Id,
            BookingId = r.BookingId,
            CustomerId = r.CustomerId,
            CustomerName = r.Customer?.FullName ?? "Unknown",
            BranchId = r.BranchId,
            OverallRating = r.OverallRating,
            CleanlinessRating = r.CleanlinessRating,
            SpeedRating = r.SpeedRating,
            StaffRating = r.StaffRating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    private async Task<ReviewResponseDto> MapToDto(string reviewId)
    {
        var review = await _context.ServiceReviews
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == reviewId);
            
        if (review == null) return null!;

        return new ReviewResponseDto
        {
            Id = review.Id,
            BookingId = review.BookingId,
            CustomerId = review.CustomerId,
            CustomerName = review.Customer?.FullName ?? "Unknown",
            BranchId = review.BranchId,
            OverallRating = review.OverallRating,
            CleanlinessRating = review.CleanlinessRating,
            SpeedRating = review.SpeedRating,
            StaffRating = review.StaffRating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<ReviewResponseDto> GetReviewByBookingIdAsync(string bookingId)
    {
        var review = await _context.ServiceReviews
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.BookingId == bookingId);

        if (review == null)
        {
            throw new Exception("Review not found for this booking.");
        }

        return await MapToDto(review.Id);
    }

    public async Task<ReviewResponseDto> UpdateReviewByBookingIdAsync(string customerId, string bookingId, CreateReviewDto dto)
    {
        var review = await _context.ServiceReviews
            .FirstOrDefaultAsync(r => r.BookingId == bookingId && r.CustomerId == customerId);

        if (review == null)
        {
            throw new Exception("Review not found or you don't have permission to update it.");
        }

        review.OverallRating = dto.OverallRating;
        review.CleanlinessRating = dto.CleanlinessRating;
        review.SpeedRating = dto.SpeedRating;
        review.StaffRating = dto.StaffRating;
        review.Comment = dto.Comment;

        await _context.SaveChangesAsync();

        return await MapToDto(review.Id);
    }

    public async Task<bool> DeleteReviewByBookingIdAsync(string customerId, string bookingId)
    {
        var review = await _context.ServiceReviews
            .FirstOrDefaultAsync(r => r.BookingId == bookingId && r.CustomerId == customerId);
            
        if (review == null)
            return false;

        _context.ServiceReviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }
}
