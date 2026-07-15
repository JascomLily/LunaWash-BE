using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateTicketStatusRequest request)
        {
            var result = await _ticketService.UpdateTicketStatusAsync(id, request.Status);
            if (!result) return NotFound("Ticket not found.");
            return Ok(new { message = "Status updated successfully." });
        }
    }

    public class UpdateTicketStatusRequest
    {
        public string Status { get; set; } = null!;
    }
}
