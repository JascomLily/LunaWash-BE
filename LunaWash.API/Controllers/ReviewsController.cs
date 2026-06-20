using System;
using System.Security.Claims;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunaWash.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
    {
        try
        {
            var customerId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId)) return Unauthorized(new { Message = "User ID not found in token." });

            var result = await _reviewService.AddReviewAsync(customerId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetReviewsByBranch(string branchId)
    {
        try
        {
            var result = await _reviewService.GetReviewsByBranchAsync(branchId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetReviewByBooking(string bookingId)
    {
        try
        {
            var result = await _reviewService.GetReviewByBookingIdAsync(bookingId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpPut("{bookingId}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(string bookingId, [FromBody] CreateReviewDto dto)
    {
        try
        {
            var customerId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId)) return Unauthorized(new { Message = "User ID not found in token." });

            var result = await _reviewService.UpdateReviewByBookingIdAsync(customerId, bookingId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{bookingId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(string bookingId)
    {
        try
        {
            var customerId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId)) return Unauthorized();

            var result = await _reviewService.DeleteReviewByBookingIdAsync(customerId, bookingId);
            if (!result) return NotFound(new { Message = "Review not found or you don't have permission to delete it." });

            return Ok(new { Message = "Review deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
