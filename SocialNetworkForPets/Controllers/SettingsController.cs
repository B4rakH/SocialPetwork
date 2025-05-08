using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUsersService _usersService;
        int UserId = 1;
        public SettingsController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public async Task<IActionResult> Index()
        {
            var userDb = await _usersService.GetUserAsync(UserId);
            
            return View(userDb);
        }
    }
}
