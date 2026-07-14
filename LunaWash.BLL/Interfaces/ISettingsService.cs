using System.Threading.Tasks;

namespace LunaWash.BLL.Interfaces
{
    public interface ISettingsService
    {
        Task<PaymentSettings> GetPaymentSettingsAsync();
        Task<bool> UpdatePaymentSettingsAsync(PaymentSettings settings);
    }

    public class PaymentSettings
    {
        public bool IsCashActive { get; set; } = true;
        public bool IsVnpayActive { get; set; } = false;
        public bool IsMomoActive { get; set; } = false;
        public bool IsZaloPayActive { get; set; } = false;
        public string VnpayTmnCode { get; set; } = "";
        public string VnpayHashSecret { get; set; } = "";
    }
}
