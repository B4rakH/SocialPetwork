using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.ViewModels.Users;

namespace SocialNetworkForPets.Controllers
{
    public class UsersController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService, AppDbContext context)
        {
            _usersService = usersService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            var userPosts = await _usersService.GetUserPostsAsync(userId);

            var IsRequestedOrAdded = IsFriend(userId);

            var userProfileVM = new GetUserProfileVM()
            {
                Posts = userPosts,
                User = user,
                RequestedOrAdded = IsRequestedOrAdded
            };

            return View(userProfileVM);
        }

        private bool IsFriend(int User1Id)
        {
            var User2Id = GetUserId();

            return (User1Id == User2Id) || (_context.Friendship.Any(u => (u.User1Id == User1Id && u.User2Id == User2Id)
                                        || (u.User1Id == User2Id && u.User2Id == User1Id)))
                                        || (_context.FriendRequests
                                    .Any(u => (u.User1Id == User1Id && u.User2Id == User2Id)
                                        || (u.User1Id == User2Id && u.User2Id == User1Id)));
        }
    }
}
