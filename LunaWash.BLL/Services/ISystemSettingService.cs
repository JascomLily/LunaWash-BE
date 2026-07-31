using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.DAL.Entities;

namespace LunaWash.BLL.Services
{
    public interface ISystemSettingService
    {
        Task<IEnumerable<SystemSetting>> GetAllSettingsAsync();
        Task<SystemSetting?> GetSettingByIdAsync(string id);
        Task<bool> UpdateSettingAsync(string id, string value);
        Task<bool> IsSettingEnabledAsync(string id);
    }
}
