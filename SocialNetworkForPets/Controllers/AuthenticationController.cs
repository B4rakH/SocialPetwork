using Microsoft.AspNetCore.Mvc;

namespace SocialNetworkForPets.Controllers
{
    public class AuthenticationController : Controller
    {
        public async Task<IActionResult> Login()
        {
            return View();
        }

        public async Task<IActionResult> Register() 
        {
            return View();
        }
    }
}
