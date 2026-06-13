using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginDto);
        Task<bool> RegisterAsync(RegisterRequestDTO registerDto);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO updateDto);
    }
}
