using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerResponseDto>> GetAllBannersAsync(bool activeOnly = false);
        Task<BannerResponseDto?> GetBannerByIdAsync(string id);
        Task<BannerResponseDto> CreateBannerAsync(BannerCreateUpdateDto dto);
        Task<bool> UpdateBannerAsync(string id, BannerCreateUpdateDto dto);
        Task<bool> DeleteBannerAsync(string id);
        Task<bool> SaveBannersBulkAsync(IEnumerable<BannerCreateUpdateDto> dtos);
    }
}
