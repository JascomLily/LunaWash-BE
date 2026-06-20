using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LunaWash.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive && !u.IsDeleted);

            if (user == null)
            {
                return null;
            }

            if (user.Password != loginDto.Password)
            {
                return null;
            }

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
                IsActive = true,
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
                var expiredRecords = await _context.PointHistories
                    .Where(p => p.UserId == user.Id && p.RemainingPoints > 0 && p.ExpiryDate <= DateTime.UtcNow && !p.IsExpired)
                    .ToListAsync();

                if (expiredRecords.Any())
                {
                    int totalExpiredPoints = 0;
                    foreach (var record in expiredRecords)
                    {
                        totalExpiredPoints += record.RemainingPoints;
                        record.IsExpired = true;
                        record.RemainingPoints = 0;
                    }

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
                Address = user.Address ?? "",
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