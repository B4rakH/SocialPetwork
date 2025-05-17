using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.ViewModels.Users;

namespace SocialNetworkForPets.Controllers
{
    public class UsersController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IUsersService _usersService;
        private readonly IFriendsService _friendsService;

        public UsersController(IUsersService usersService,
                AppDbContext context, IFriendsService friendsService,
                    IHashtagService hashtagService)
        {
            _usersService = usersService;
            _context = context;
            _friendsService = friendsService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int userId)
        {
            var loggedUserId = GetUserId();
            if (loggedUserId == null) return RedirectToLogin();

            var user = await _context.User.FindAsync(userId);
            var userPosts = await _usersService.GetUserPostsAsync(userId);
            var friendships = await _friendsService.GetFriendsAsync(userId);

            var IsRequestedOrAdded = IsFriend(userId, loggedUserId.Value);

            var userProfileVM = new GetUserProfileVM()
            {
                Posts = userPosts,
                User = user,
                RequestedOrAdded = IsRequestedOrAdded,
                Friendships = friendships
            };

            return View(userProfileVM);
        }

        private bool IsFriend(int User1Id, int User2Id)
        {

            return (User1Id == User2Id) || (_context.Friendship.Any(u => (u.User1Id == User1Id && u.User2Id == User2Id)
                                        || (u.User1Id == User2Id && u.User2Id == User1Id)))
                                        || (_context.FriendRequests
                                    .Any(u => (u.SenderId == User1Id && u.ReceiverId == User2Id)
                                        || (u.SenderId == User2Id && u.ReceiverId == User1Id)));
        }
    }
}
