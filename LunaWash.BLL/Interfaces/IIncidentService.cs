using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IIncidentService
    {
        Task<IncidentReportResponseDto?> ReportIncidentAsync(string branchId, string reporterId, IncidentReportCreateDto dto);
        Task<IEnumerable<IncidentReportResponseDto>> GetIncidentsByBranchAsync(string branchId);
        Task<bool> ApproveIncidentAsync(string id, string assignedToId, string priority);
        Task<bool> RejectIncidentAsync(string id);
    }
}
