using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Helper.Constants;
using SocialNetworkForPets.Hubs;

namespace SocialNetworkForPets.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(AppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task AddNewNotificationAsync(int UserId, string type, string userFullName, int? postId = null)
        {
            var newNotification = new Notification()
            {
                UserId = UserId,
                Message = GetPostMessage(type, userFullName),
                Type = type,
                PostId = postId
            };

            await _context.Notification.AddAsync(newNotification);

            await _context.SaveChangesAsync();
            
            var notificationCount = await GetNotificationsCountAsync(UserId);

            //sends notification count data to the notification button

            await _hubContext.Clients.User(UserId.ToString())
                .SendAsync("ReceiveNotification", notificationCount);

        }

        public async Task<int> GetNotificationsCountAsync(int UserId)
        {
            var count = await _context.Notification
                .Where(n => n.UserId == UserId)
                .CountAsync();
            return count;
        }

        public async Task<List<Notification>> GetNotifications(int UserId)
        {
            var allNotifications = await _context.Notification
                .Where(n => n.UserId == UserId)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();

            return allNotifications;
        }

        public async Task DeleteNotification(int notificationId)
        {
            var notification = await _context.Notification.FirstOrDefaultAsync(n => n.Id == notificationId);
            if (notification == null) return;
            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();
        }

        private string GetPostMessage(string notificationType, string userFullName)
        {
            switch (notificationType)
            {
                case NotificationType.Like:
                    return $"{userFullName} liked your post";

                    
                case NotificationType.Comment:
                    return $"{userFullName} added a coment to your post";
                    

                case NotificationType.FriendRequest:
                    return $"{userFullName} sent you friend request";

                case NotificationType.Favorite:
                    return $"{userFullName} favorited your post";


                case NotificationType.FriendRequestApproved:
                    return $"{userFullName} approved your friendship request";


                default:
                    return "";
            }
        }
    }
}
