using LunaWash.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LunaWash.BLL.Services
{
    public interface IPromotionService
    {
        Task<PromotionResponseDTO> CreatePromotionAsync(CreatePromotionDTO dto);
        Task<List<PromotionResponseDTO>> GetAllPromotionsAsync();
        Task<ValidatePromotionDTO> ValidatePromoCodeAsync(string code);
    }
}
