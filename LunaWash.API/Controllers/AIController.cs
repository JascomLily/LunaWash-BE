using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        
        private const string SystemInstruction = @"Bạn là Luna AI Assistant - Trợ lý hỗ trợ khách hàng thông minh của hệ thống rửa xe cao cấp LunaWash.
MẪU XỬ LÝ TÌNH HUỐNG (FEW-SHOT EXAMPLES):
- Khách: ""Thời tiết hôm nay ở Sài Gòn thế nào?""
- Trợ lý: Dạ, em là Trợ lý ảo chuyên hỗ trợ **đặt lịch rửa và chăm sóc xe**. Em không thể hỗ trợ các thông tin ngoài dịch vụ này ạ. Anh/chị có cần em tư vấn gói rửa xe hoặc hỗ trợ đặt lịch ngay không ạ?
- Khách: ""Viết giúp mình đoạn code C# gọi API""
- Trợ lý: Dạ, em là Trợ lý ảo chuyên hỗ trợ **đặt lịch rửa và chăm sóc xe**. Em không thể hỗ trợ các thông tin ngoài dịch vụ này ạ. Anh/chị có cần em tư vấn gói rửa xe hoặc hỗ trợ đặt lịch ngay không ạ?
- Khách: ""Bên mình rửa xe ô tô 4 chỗ hết bao nhiêu tiền và mất bao lâu?""
- Trợ lý: Dạ, mức giá và thời gian của các gói dịch vụ có thể được cập nhật thường xuyên. Anh/chị vui lòng truy cập trang **Đặt lịch (Booking)** để xem chi tiết từng gói và mức giá chính xác nhất hôm nay nhé! Anh/chị có cần em hướng dẫn cách vào trang Đặt lịch không ạ?";

        public AIController(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"] ?? string.Empty;
            _httpClient = new HttpClient();
        }

        public class ChatRequest
        {
            public List<dynamic> Contents { get; set; } = new List<dynamic>();
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                return StatusCode(500, new { error = "Gemini API Key is not configured." });
            }

            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={_apiKey}";

            var payload = new
            {
                systemInstruction = new
                {
                    parts = new[] { new { text = SystemInstruction } }
                },
                contents = request.Contents
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(apiUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse the Gemini response to extract the text
                    using var document = JsonDocument.Parse(responseString);
                    var candidates = document.RootElement.GetProperty("candidates");
                    if (candidates.GetArrayLength() > 0)
                    {
                        var text = candidates[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();
                        
                        return Ok(new { text });
                    }
                    
                    return Ok(new { text = "Không có phản hồi từ AI." });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { error = "Lỗi kết nối AI: " + responseString });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
