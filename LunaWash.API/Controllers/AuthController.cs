using System.Security.Claims;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng đăng nhập cho người dùng
        //<</.....>>
        // [API: POST /api/Auth/login, dùng để xử lý đăng nhập, nhận dữ liệu đăng nhập từ FE]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.LoginAsync(loginDto);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                return Ok(result);
            }
            catch (System.UnauthorizedAccessException ex) when (ex.Message == "EmailNotVerified")
            {
                return StatusCode(403, new { message = "EmailNotVerified" });
            }
            catch (System.UnauthorizedAccessException ex) when (ex.Message == "GoogleLoginRequired")
            {
                return BadRequest(new { message = "Tài khoản này được đăng ký bằng Google. Vui lòng bấm nút Đăng nhập với Google." });
            }
        }

        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng đăng ký tài khoản mới cho người dùng
        //<</.....>>
        // [API: POST /api/Auth/register, dùng để đăng ký, nhận thông tin đăng ký từ FE]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result)
            {
                return BadRequest(new { message = "Registration failed. Email might already exist." });
            }

            return Ok(new { message = "Registration successful." });
        }

        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng đăng nhập bằng Google
        //<</.....>>
        // [API: POST /api/Auth/google-login, dùng để đăng nhập qua Google, nhận token từ FE]
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clientId = _configuration["Google:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                return StatusCode(500, new { message = "Server is missing Google ClientId configuration." });
            }

            try {
                var result = await _authService.GoogleLoginAsync(request.Token, clientId);
                if (result == null)
                {
                    return BadRequest(new { message = "Đăng nhập Google thất bại. Token không hợp lệ." });
                }
                return Ok(result);
            } catch (Exception ex) {
                return BadRequest(new { message = "LỖI TỪ BE: " + ex.Message });
            }
        }

        public class VerifyOtpRequest
        {
            public string Email { get; set; } = null!;
            public string Otp { get; set; } = null!;
        }

        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng xác thực mã OTP
        //<</.....>>
        // [API: POST /api/Auth/verify-otp, dùng để xác thực OTP, nhận email và mã OTP từ FE]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _authService.VerifyOtpAsync(request.Email, request.Otp);
            if (result)
            {
                return Ok(new { message = "Xác thực email thành công." });
            }
            return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
        }

        public class ResendOtpRequest
        {
            public string Email { get; set; } = null!;
        }

        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng gửi lại mã OTP
        //<</.....>>
        // [API: POST /api/Auth/resend-otp, dùng để gửi lại OTP, nhận email từ FE]
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var result = await _authService.SendOtpAsync(request.Email);
            if (result)
            {
                return Ok(new { message = "Đã gửi lại mã OTP." });
            }
            return BadRequest(new { message = "Email không tồn tại trong hệ thống." });
        }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; } = null!;
        }

        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng quên mật khẩu
        //<</.....>>
        // [API: POST /api/Auth/forgot-password, dùng để gửi OTP quên mật khẩu, nhận email từ FE]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.SendOtpAsync(request.Email, "forgot-password");
            if (result)
            {
                return Ok(new { message = "Mã OTP đã được gửi." });
            }
            return BadRequest(new { message = "Email không tồn tại trong hệ thống." });
        }

        public class ResetPasswordRequest
        {
            public string Email { get; set; } = null!;
            public string Otp { get; set; } = null!;
            public string NewPassword { get; set; } = null!;
        }

        //<<Comment Function>>
        // Hàm này là: Xử lý chức năng đặt lại mật khẩu mới
        //<</.....>>
        // [API: POST /api/Auth/reset-password, dùng để đặt lại mật khẩu, nhận email, OTP và mật khẩu mới từ FE]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request.Email, request.Otp, request.NewPassword);
            if (result)
            {
                return Ok(new { message = "Đặt lại mật khẩu thành công." });
            }
            return BadRequest(new { message = "Mã OTP không hợp lệ hoặc đã hết hạn." });
        }

        [Authorize]
        //<<Comment Function>>
        // Hàm này là: Lấy thông tin của người dùng hiện tại đang đăng nhập
        //<</.....>>
        // [API: GET /api/Auth/me, dùng để lấy thông tin tài khoản, yêu cầu token hợp lệ từ FE]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userProfile = await _authService.GetUserProfileAsync(userId);
            
            if (userProfile == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin người dùng." });
            }

            return Ok(userProfile);
        }

        [Authorize]
        //<<Comment Function>>
        // Hàm này là: Cập nhật thông tin hồ sơ của người dùng
        //<</.....>>
        // [API: PUT /api/Auth/me, dùng để cập nhật profile, nhận dữ liệu cập nhật từ FE]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy ID người dùng từ JWT Token
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var success = await _authService.UpdateProfileAsync(userId, updateDto);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            return Ok(new { message = "Đã cập nhật thông tin thành công." });
        }

        [Authorize]
        //<<Comment Function>>
        // Hàm này là: Kiểm tra trạng thái đăng nhập của người dùng
        //<</.....>>
        // [API: GET /api/Auth/check, dùng để kiểm tra login, trả về true nếu token hợp lệ]
        [HttpGet("check")]
        public IActionResult CheckLogin()
        {
            return Ok(new { isAuthenticated = true });
        }
    }
}