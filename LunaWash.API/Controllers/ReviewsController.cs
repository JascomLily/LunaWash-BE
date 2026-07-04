using System;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                ?? User?.FindFirst("sub")?.Value 
                ?? string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var review = await _reviewService.CreateReviewAsync(userId, dto);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetReviewByBookingId(string bookingId)
        {
            var review = await _reviewService.GetReviewByBookingIdAsync(bookingId);
            if (review == null)
                return NotFound();
            return Ok(review);
        }

        [HttpPut("{bookingId}")]
        public async Task<IActionResult> UpdateReview(string bookingId, [FromBody] UpdateReviewDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var review = await _reviewService.UpdateReviewAsync(userId, bookingId, dto);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteReview(string bookingId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var result = await _reviewService.DeleteReviewAsync(userId, bookingId);
                if (result)
                    return Ok(new { message = "Review deleted successfully" });
                return BadRequest(new { message = "Failed to delete review" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("branch/{branchId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByBranch(string branchId)
        {
            var reviews = await _reviewService.GetReviewsByBranchAsync(branchId);
            return Ok(reviews);
        }

        [HttpPut("{id}/respond")]
        public async Task<IActionResult> RespondToReview(string id, [FromBody] ReviewRespondDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var success = await _reviewService.RespondToReviewAsync(id, userId, dto.ResponseText);
                if (success)
                    return Ok(new { message = "Đã phản hồi đánh giá thành công." });
                return BadRequest(new { message = "Không tìm thấy đánh giá." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
