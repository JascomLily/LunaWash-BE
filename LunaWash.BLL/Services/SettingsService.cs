using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LunaWash.BLL.Interfaces;

namespace LunaWash.BLL.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath = "payment_settings.json";

        public async Task<PaymentSettings> GetPaymentSettingsAsync()
        {
            if (!File.Exists(_settingsFilePath))
            {
                var defaultSettings = new PaymentSettings();
                await UpdatePaymentSettingsAsync(defaultSettings);
                return defaultSettings;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<PaymentSettings>(json);
                return settings ?? new PaymentSettings();
            }
            catch
            {
                return new PaymentSettings();
            }
        }

        public async Task<bool> UpdatePaymentSettingsAsync(PaymentSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_settingsFilePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
