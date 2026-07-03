using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Caching.Memory;

namespace LunaWash.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, IMemoryCache cache, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
            _cache = cache;
            _emailService = emailService;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && !u.IsDeleted);

            if (user == null)
            {
                return null;
            }

            // Chặn đăng nhập bằng form thường nếu tài khoản tạo bằng Google
            if (user.Password == "GOOGLE_OAUTH_LOGIN")
            {
                throw new UnauthorizedAccessException("GoogleLoginRequired");
            }

            if (user.Password != loginDto.Password)
            {
                return null;
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("EmailNotVerified");
            }

            // Retrieve Tier for Customer
            string tierName = "Đồng";
            if (user.Role.RoleName == "Customer")
            {
                var profile = await _context.CustomerProfiles
                    .Include(cp => cp.MembershipTier)
                    .FirstOrDefaultAsync(cp => cp.UserId == user.Id);

                if (profile != null && profile.MembershipTier != null)
                {
                    tierName = profile.MembershipTier.TierName;
                    if (tierName == "Member") tierName = "Đồng";
                }
            }

            var token = GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.RoleName,
                Tier = tierName,
                BranchId = user.BranchId
            };
        }

        public async Task<LoginResponseDTO?> GoogleLoginAsync(string googleToken, string clientId)
        {
            try
            {
                // Validate Google Token
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { clientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
                
                string userEmail = payload.Email;

                var existingUser = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == userEmail && !u.IsDeleted);

                if (existingUser == null)
                {
                    var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer");
                    if (customerRole == null) return null;

                    existingUser = new User
                    {
                        Id = "USR-" + DateTime.UtcNow.ToString("yyMM") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper(),
                        FullName = payload.Name ?? "Google User",
                        Email = userEmail,
                        RoleId = customerRole.Id,
                        Role = customerRole,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false,
                        PhoneNumber = "Chưa cập nhật",
                        Address = "",
                        Password = "GOOGLE_OAUTH_LOGIN" // Dummy password
                    };

                    _context.Users.Add(existingUser);

                    var defaultTier = await _context.MembershipTiers.FirstOrDefaultAsync(t => t.Id == "TIER-MEM");
                    if (defaultTier != null)
                    {
                        var profile = new CustomerProfile
                        {
                            UserId = existingUser.Id,
                            CurrentPoints = 0,
                            AccumulatedPoints = 0,
                            MembershipTierId = defaultTier.Id
                        };
                        _context.CustomerProfiles.Add(profile);
                    }

                    await _context.SaveChangesAsync();
                }

                // Retrieve Tier Name for Response
                string tierName = "Đồng";
                if (existingUser.Role?.RoleName == "Customer")
                {
                    var profile = await _context.CustomerProfiles
                        .Include(cp => cp.MembershipTier)
                        .FirstOrDefaultAsync(cp => cp.UserId == existingUser.Id);

                    if (profile != null && profile.MembershipTier != null)
                    {
                        tierName = profile.MembershipTier.TierName;
                        if (tierName == "Member") tierName = "Đồng";
                    }
                }

                var token = GenerateJwtToken(existingUser);

                bool needsUpdate = string.IsNullOrEmpty(existingUser.PhoneNumber) || existingUser.PhoneNumber == "Chưa cập nhật";

                return new LoginResponseDTO
                {
                    Token = token,
                    FullName = existingUser.FullName,
                    Email = existingUser.Email,
                    Role = existingUser.Role.RoleName,
                    Tier = tierName,
                    BranchId = existingUser.BranchId,
                    RequiresProfileUpdate = needsUpdate
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("GOOGLE LOGIN ERROR: " + ex.ToString());
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequestDTO registerDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return false;
            }

            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Customer");
            if (customerRole == null) return false;

            var user = new User
            {
                Id = "USR-" + DateTime.UtcNow.ToString("yyMM") + "-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper(),
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.Phone,
                RoleId = customerRole.Id,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            user.Password = registerDto.Password;
            _context.Users.Add(user);

            if (customerRole != null && customerRole.Id == user.RoleId)
            {
                var defaultTier = await _context.MembershipTiers.FirstOrDefaultAsync(t => t.Id == "TIER-MEM");
                if (defaultTier != null)
                {
                    var profile = new CustomerProfile
                    {
                        UserId = user.Id,
                        CurrentPoints = 0,
                        AccumulatedPoints = 0,
                        MembershipTierId = defaultTier.Id
                    };
                    _context.CustomerProfiles.Add(profile);
                }
            }

            await _context.SaveChangesAsync();
            await SendOtpAsync(user.Email);
            return true;
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO updateDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.FullName = updateDto.FullName;
            user.PhoneNumber = updateDto.Phone;
            user.Address = updateDto.Address;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendOtpAsync(string email, string purpose = "register")
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            var otp = new Random().Next(100000, 999999).ToString();
            _cache.Set($"OTP_{email}", otp, TimeSpan.FromMinutes(5));

            string message = purpose == "register" 
                ? "Cảm ơn bạn đã đăng ký tài khoản tại LunaWash. Mã xác thực (OTP) của bạn là:"
                : "Bạn vừa yêu cầu lấy lại mật khẩu tại LunaWash. Mã xác thực (OTP) của bạn là:";
                
            string title = purpose == "register"
                ? "Mã Xác Thực Đăng Ký Tài Khoản - LunaWash"
                : "Mã Xác Thực Lấy Lại Mật Khẩu - LunaWash";

            string emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e1e1e1; border-radius: 10px;'>
                    <h2 style='color: #4F46E5; text-align: center;'>LunaWash</h2>
                    <p>Xin chào <strong>{user.FullName}</strong>,</p>
                    <p>{message}</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #1F2937; background-color: #F3F4F6; padding: 15px 30px; border-radius: 8px;'>{otp}</span>
                    </div>
                    <p style='color: #6B7280; font-size: 14px;'>Mã OTP này sẽ hết hạn sau <strong>5 phút</strong>. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                    <hr style='border: none; border-top: 1px solid #e1e1e1; margin: 30px 0;' />
                    <p style='color: #9CA3AF; font-size: 12px; text-align: center;'>Đây là email tự động, vui lòng không trả lời.</p>
                </div>";

            await _emailService.SendEmailAsync(email, title, emailBody);
            return true;
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            if (_cache.TryGetValue($"OTP_{email}", out string cachedOtp))
            {
                if (cachedOtp == otp)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user != null)
                    {
                        user.IsActive = true;
                        await _context.SaveChangesAsync();
                        _cache.Remove($"OTP_{email}");
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            if (_cache.TryGetValue($"OTP_{email}", out string cachedOtp))
            {
                if (cachedOtp == otp)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user != null)
                    {
                        user.Password = newPassword;
                        user.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        _cache.Remove($"OTP_{email}");
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<UserProfileResponseDTO?> GetUserProfileAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.CustomerProfile)
                    .ThenInclude(cp => cp!.MembershipTier)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null) return null;

            LoyaltyInfoDTO? loyaltyInfo = null;
            if (user.CustomerProfile != null)
            {
                // ---------------- ĐOẠN CODE KIỂM TRA ĐIỂM HẾT HẠN 1 NĂM ----------------
                // Tìm những đợt cộng điểm nào đã quá 1 năm (ExpiryDate <= Hiện tại) nhưng chưa xử lý hết hạn
                var expiredRecords = await _context.PointHistories
                    .Where(p => p.UserId == user.Id && p.RemainingPoints > 0 && p.ExpiryDate <= DateTime.UtcNow && !p.IsExpired)
                    .ToListAsync();

                if (expiredRecords.Any())
                {
                    int totalExpiredPoints = 0;
                    foreach (var record in expiredRecords)
                    {
                        totalExpiredPoints += record.RemainingPoints;
                        record.IsExpired = true;       // Đánh dấu đã xử lý hết hạn
                        record.RemainingPoints = 0;    // Đưa số lượng điểm khả dụng về 0
                    }

                    // Ghi log hệ thống thu hồi điểm hết hạn
                    var expireLog = new PointHistory
                    {
                        UserId = user.Id,
                        Points = -totalExpiredPoints,
                        RemainingPoints = 0,
                        Description = $"Hệ thống tự động thu hồi {totalExpiredPoints} điểm thưởng do hết hạn 1 năm",
                        CreatedAt = DateTime.UtcNow,
                        ExpiryDate = null
                    };
                    _context.PointHistories.Add(expireLog);

                    // Trừ thẳng vào điểm hiện tại của khách hàng (đảm bảo không âm)
                    user.CustomerProfile.CurrentPoints = Math.Max(0, user.CustomerProfile.CurrentPoints - totalExpiredPoints);
                    await _context.SaveChangesAsync();
                }
                // ----------------------------------------------------------------------

                loyaltyInfo = new LoyaltyInfoDTO
                {
                    CurrentPoints = user.CustomerProfile.CurrentPoints,
                    AccumulatedPoints = user.CustomerProfile.AccumulatedPoints,
                    TierName = user.CustomerProfile.MembershipTier?.TierName ?? "Member",
                    DiscountPercent = user.CustomerProfile.MembershipTier?.DiscountPercent ?? 0
                };
            }

            return new UserProfileResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.RoleName,
                Phone = user.PhoneNumber ?? "",
                Address = user.Address,
                IsActive = user.IsActive,
                Loyalty = loyaltyInfo
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}