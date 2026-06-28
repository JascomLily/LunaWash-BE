using System.Threading.Tasks;
using LunaWash.BLL.DTOs;

namespace LunaWash.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginDto);
        Task<LoginResponseDTO?> GoogleLoginAsync(string googleToken, string clientId);
        Task<bool> RegisterAsync(RegisterRequestDTO registerDto);
        Task<UserProfileResponseDTO?> GetUserProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO updateDto);
        Task<bool> SendOtpAsync(string email, string purpose = "register");
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);
    }
}