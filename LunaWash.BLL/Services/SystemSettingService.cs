using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;

namespace LunaWash.BLL.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SystemSetting>> GetAllSettingsAsync()
        {
            return await _context.SystemSettings.ToListAsync();
        }

        public async Task<SystemSetting?> GetSettingByIdAsync(string id)
        {
            return await _context.SystemSettings.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> UpdateSettingAsync(string id, string value)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Id == id);
            if (setting == null)
            {
                setting = new SystemSetting
                {
                    Id = id,
                    Value = value,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsSettingEnabledAsync(string id)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Id == id);
            if (setting == null) return false;
            return string.Equals(setting.Value, "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
