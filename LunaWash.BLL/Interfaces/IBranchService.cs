using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchResponseDto>> GetAllBranchesAsync(bool activeOnly = false);
        Task<BranchResponseDto?> GetBranchByIdAsync(string id);
        Task<BranchResponseDto> CreateBranchAsync(BranchCreateDto dto);
        Task<BranchResponseDto?> UpdateBranchAsync(string id, BranchUpdateDto dto);
        Task<bool> DeleteBranchAsync(string id);
        Task<IEnumerable<BranchEquipmentDto>> GetEquipmentsByBranchAsync(string branchId);
        Task<IEnumerable<BranchSlotDto>> GetSlotsByBranchAsync(string branchId);
    }
}
