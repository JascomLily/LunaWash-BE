using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IMembershipTierService
    {
        Task<IEnumerable<MembershipTierResponseDto>> GetAllTiersAsync();
        Task<MembershipTierResponseDto?> GetTierByIdAsync(string id);
        Task<MembershipTierResponseDto> CreateTierAsync(string customId, MembershipTierCreateUpdateDto dto);
        Task<bool> UpdateTierAsync(string id, MembershipTierCreateUpdateDto dto);
        Task<bool> DeleteTierAsync(string id);
        Task<bool> AdjustCustomerPointsAsync(AdjustPointsDto dto);
        Task<IEnumerable<PointHistoryResponseDto>> GetCustomerPointHistoryAsync(string userId);
    }
}
