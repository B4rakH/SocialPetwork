using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Authentication;

namespace SocialNetworkForPets.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AppDbContext _context;
        public AuthenticationController(AppDbContext context) 
        {
            _context = context;
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }

        public async Task<IActionResult> Register() 
        {
            return View();
        }

        public async Task<IActionResult> Register(RegisterVM registerVM)
        {

            return View();
        }
    }
}
