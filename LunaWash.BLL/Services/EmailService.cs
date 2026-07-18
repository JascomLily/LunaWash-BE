using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using LunaWash.BLL.Interfaces;

namespace LunaWash.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        
        // This is the Google Apps Script Web App URL to bypass Render's SMTP block
        private readonly string _googleScriptUrl = "https://script.google.com/macros/s/AKfycbyRNYzBDpUlH3HMLcvr6r29ibJD8KvlS6SMZiX9dqZECM2cYp8FvVEb1aw_1Spbv0DX/exec";

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var payload = new
            {
                to = toEmail,
                subject = subject,
                body = body
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                // Send via HTTP POST over port 443 which is never blocked
                var response = await _httpClient.PostAsync(_googleScriptUrl, content);
                response.EnsureSuccessStatusCode();
                
                var resultString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Google Script Email Response: " + resultString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email via Google Script: " + ex.Message);
                throw;
            }
        }
    }
}
