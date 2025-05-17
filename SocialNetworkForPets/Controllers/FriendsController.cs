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

        public async Task<IActionResult> RejectFriendRequest(int senderId)
        {
            var receiverId = GetUserId();
            if(receiverId == null) return RedirectToLogin();

            await _friendsService.RejectRequestAsync(senderId, receiverId.Value);

            return RedirectToAction("Index");
        }
        [HttpPost]

        public async Task<IActionResult> CancelFriendRequest(int receiverId)
        {
            var senderId = GetUserId();
            if (senderId == null) return RedirectToLogin();

            await _friendsService.RejectRequestAsync(senderId.Value, receiverId);

            return RedirectToAction("Index");
        }

        [HttpPost]

        public async Task<IActionResult> AcceptFriendRequest(int senderId)
        {
            var receiverId = GetUserId();
            var fullName = GetUserFullName();
            if (receiverId == null) return RedirectToLogin();

            var request = await _context.FriendRequests.FirstAsync(r => r.SenderId == senderId && r.ReceiverId == receiverId.Value);

            await _notificationService.AddNewNotificationAsync
                (senderId, NotificationType.FriendRequestApproved, fullName);

            await _friendsService.AcceptRequestAsync(senderId, receiverId.Value);

            return RedirectToAction("Index");
        }

        [HttpPost]

        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToLogin();

            await _friendsService.RemoveFriendAsync(userId.Value ,friendId);

            return RedirectToAction("Index");
        }
    }
}
