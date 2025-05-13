using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Helper.Constants;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.ViewModels.Friends;

namespace SocialNetworkForPets.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IFriendsService _friendsService;
        private readonly INotificationService _notificationService;

        public FriendsController(IFriendsService friendsService
            , INotificationService notificationService
                , AppDbContext context)
        {
            _friendsService = friendsService;
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToLogin();

            var friendRequestData = new FriendshipVM()
            {
                Friends = await _friendsService.GetFriendsAsync(userId.Value),
                FriendRequestsSent = await _friendsService.GetSentFriendRequestAsync(userId.Value),
                FriendRequestsReceived = await _friendsService.GetReceivedFriendRequestAsync(userId.Value),
            };

            return View(friendRequestData);
        }
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int receiverId)
        {
            var userId = GetUserId();
            var fullName = GetUserFullName();
            if (userId == null) return RedirectToLogin();

            await _friendsService.SendRequestAsync(userId.Value, receiverId);

            await _notificationService.AddNewNotificationAsync
                (receiverId, NotificationType.FriendRequest, fullName);

            return RedirectToAction("Index","Home");
        }
        [HttpPost]

        public async Task<IActionResult> RejectFriendRequest(int requestId)
        {
            await _friendsService.RejectRequestAsync(requestId);

            return RedirectToAction("Index");
        }
        [HttpPost]

        public async Task<IActionResult> AcceptFriendRequest(int requestId)
        {
            var userId = GetUserId();
            var fullName = GetUserFullName();
            if (userId == null) return RedirectToLogin();

            var request = await _context.FriendRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);

            await _notificationService.AddNewNotificationAsync
                (request.User1Id, NotificationType.FriendRequestApproved, fullName);

            await _friendsService.AcceptRequestAsync(requestId);

            return RedirectToAction("Index");
        }

        [HttpPost]

        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            await _friendsService.RemoveFriendAsync(friendshipId);

            return RedirectToAction("Index");
        }
    }
}
