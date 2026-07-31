using System.Text;
using Microsoft.EntityFrameworkCore;
using LunaWash.BLL;
using LunaWash.BLL.Services;
using LunaWash.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// Khởi tạo Builder < Tạo lõi ứng dụng web API > <Rất quan trọng>
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Đăng ký Controller < Cho phép API nhận các Request HTTP (GET, POST...) > <Rất quan trọng>
builder.Services.AddControllers();

// Khám phá API < Hỗ trợ Swagger đọc các đường dẫn API > <Bình thường>
builder.Services.AddEndpointsApiExplorer();

// Đăng ký Service Review < Chứa logic xử lý Đánh giá > <Quan trọng>
builder.Services.AddScoped<IReviewService, ReviewService>();

// Cấu hình Bộ nhớ tạm < Lưu dữ liệu đệm (Cache) trên RAM server để load nhanh > <Bình thường>
builder.Services.AddMemoryCache();

// Đăng ký Service Email < Chứa logic gửi Mail bằng MailKit > <Quan trọng>
builder.Services.AddScoped<IEmailService, EmailService>();

// Đăng ký Service Thông báo < Chứa logic gửi thông báo > <Quan trọng>
builder.Services.AddScoped<INotificationService, NotificationService>();

// Chạy ngầm dọn dẹp < Xóa/hủy các lịch đặt quá hạn 10p chưa thanh toán > <Rất quan trọng>
builder.Services.AddHostedService<LunaWash.API.BackgroundServices.BookingCleanupService>();

// Chạy ngầm đẩy ưu tiên < Tự động tăng mức độ ưu tiên cho các sự cố kỹ thuật > <Quan trọng>
builder.Services.AddHostedService<LunaWash.API.HostedServices.PriorityEscalationService>();

// Thêm Swagger < Tạo giao diện danh sách API để test code > <Bình thường>
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
// Lấy cấu hình JWT < Lấy khóa bí mật trong appsettings để mã hóa Token đăng nhập > <Rất quan trọng>
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? "DefaultSuperSecretKeyForDevelopmentOnly123!";

// Xác thực JWT < Kiểm tra user có truyền Token hợp lệ không mỗi khi gọi API > <Rất quan trọng>
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Register BLL and DAL dependencies
// Gọi hàm AddBusinessLogicLayer < Đăng ký ConnectionString và toàn bộ logic gọi Database > <Cực kỳ quan trọng>
builder.Services.AddBusinessLogicLayer(builder.Configuration);

// Add CORS if needed for frontend connection
// Cấu hình CORS < Mở cửa cho phép Frontend (Web React) kết nối lấy dữ liệu > <Rất quan trọng>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Đóng gói Builder < Khởi chạy ứng dụng với các cấu hình ở trên > <Rất quan trọng>
var app = builder.Build();

// Migrate DB < Tự động cập nhật các bảng vào Database SQL mỗi khi chạy server > <Rất quan trọng>
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<LunaWash.DAL.Data.ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("[Migration Warning] Database Migration skipped or encountered warning: " + ex.Message);
    }
}

// Configure the HTTP request pipeline.
// Chạy Swagger < Mở giao diện API khi ở môi trường Code (Dev) > <Bình thường>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Bật CORS < Kích hoạt chính sách mở cửa AllowAll đã đăng ký ở trên > <Rất quan trọng>
app.UseCors("AllowAll");

// Bật Xác thực & Phân quyền < Chặn các API yêu cầu đăng nhập > <Rất quan trọng>
app.UseAuthentication(); // Important: UseAuthentication must be before UseAuthorization
app.UseAuthorization();

// Bật đường dẫn API < Khớp các URL web với code C# tương ứng trong Controllers > <Rất quan trọng>
app.MapControllers();

// Chạy Server < Bắt đầu lắng nghe Request từ FE > <Rất quan trọng>
app.Run();
