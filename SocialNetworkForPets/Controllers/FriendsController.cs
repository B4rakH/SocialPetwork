using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.ViewModels.Friends;

namespace SocialNetworkForPets.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly IFriendsService _friendsService;

        public FriendsController(IFriendsService friendsService)
        {
            _friendsService = friendsService;
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
            if (userId == null) return RedirectToLogin();

            await _friendsService.SendRequestAsync(userId.Value, receiverId);

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
