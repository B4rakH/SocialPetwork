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

            var userProfileVM = new GetUserProfileVM()
            {
                Posts = userPosts,
                User = user
            };

            return View(userProfileVM);
        }
    }
}
