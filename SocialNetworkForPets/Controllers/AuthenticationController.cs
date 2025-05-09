using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Authentication;
using System.Reflection.Metadata;

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

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);

            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.UserName == registerVM.UserName);
            
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "Username already exists");
                return View(registerVM);
            }

            var newUser = new User()
            {
                UserFullName = $"{registerVM.FirstName} {registerVM.LastName}",
                UserName = registerVM.UserName,
                UserPassword = registerVM.Password,
                UserRank = GetUserRank(registerVM.UserName.ToLower())
            };

            await _context.User.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        private string GetUserRank(string username)
        {
            if (username.Contains("@admin")) return "Catmin";

            else if (username.Contains("@moderator")) return "Moderadog";

            else return "Default";
        }
    }
}
