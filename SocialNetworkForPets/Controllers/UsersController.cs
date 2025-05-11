using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;

namespace SocialNetworkForPets.Controllers
{
    public class UsersController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
