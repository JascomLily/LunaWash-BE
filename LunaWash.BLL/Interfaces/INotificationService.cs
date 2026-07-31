using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.DAL.Entities;

namespace LunaWash.BLL.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string title, string message, string type);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(string notificationId);
    }
}
