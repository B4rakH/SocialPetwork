using SocialNetworkForPets.Data.Models;

namespace SocialNetworkForPets.Services
{
    public interface INotificationService
    {
        Task AddNewNotificationAsync(int UserId, string Message, string userFullName, int? postId = null);

        Task<int> GetNotificationsCountAsync(int UserId);

        Task<List<Notification>> GetNotifications(int UserId);

        Task DeleteNotification(int notificationId);
    }
}
