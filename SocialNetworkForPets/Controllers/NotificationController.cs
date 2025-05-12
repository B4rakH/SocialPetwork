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

            //Will redirect count data to the JS of Home/Index for reviewing with signalR
            return Json(count);
        }
    }
}
