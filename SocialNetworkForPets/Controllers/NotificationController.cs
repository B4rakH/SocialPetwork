using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var count = await _notificationService.GetNotificationsCountAsync(UserId.Value);

            //Will redirect notification count data to the script of Home/Index for reviewing with signalR, real-time retrievement
            return Json(count);
        }
        public async Task<IActionResult> GetNotifications()
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var notifications = await _notificationService.GetNotifications(UserId.Value);
            
            return PartialView("Notifications/_Notifications", notifications);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            await _notificationService.DeleteNotification(notificationId);

            var notifications = await _notificationService.GetNotifications(UserId.Value);
            
            return PartialView("Notifications/_Notifications", notifications);
        }
    }
}
