using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunaWash.BLL.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
        Task<bool> UpdateTicketStatusAsync(string id, string status);
    }

    public class TicketDto
    {
        public string Id { get; set; } = null!;
        public string BranchName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string RequesterName { get; set; } = null!;
        public string Time { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
