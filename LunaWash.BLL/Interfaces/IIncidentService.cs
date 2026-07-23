using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IIncidentService
    {
        Task<IncidentResponse> CreateIncidentAsync(CreateIncidentRequest request, string reporterId);
        Task<IEnumerable<IncidentResponse>> GetIncidentsByBranchAsync(string branchId);
        Task<IncidentResponse?> GetIncidentByIdAsync(string incidentId);
        Task<bool> UpdateIncidentStatusAsync(string incidentId, string status);
    }
}
