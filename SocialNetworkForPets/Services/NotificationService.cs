using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
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

        public async Task AddNewNotificationAsync(int userId, string message, string type)
        {
            var newNotification = new Notification()
            {
                UserId = userId,
                Message = message,
                Type = type
            };

            var notificationCount = await GetNotificationsCountAsync(userId);

            //sends notification count data to the notification button

            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", notificationCount);

            await _context.Notification.AddAsync(newNotification);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetNotificationsCountAsync(int UserId)
        {
            var count = await _context.Notification
                .Where(n => n.UserId == UserId)
                .CountAsync();
            return count;
        }
    }
}
