using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Authentication;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using SocialNetworkForPets.Helper.Constants;
using SocialNetworkForPets.ViewModels.Settings;

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
            if (!ModelState.IsValid) return View(registerVM);

            var existingUser = await _context.User.FirstOrDefaultAsync(u => (u.UserName == registerVM.UserName));

            var userRank = GetUserRank(registerVM.UserName);

            var existingAdmin = (userRank == "Catmin") ?
                await _context.User.FirstOrDefaultAsync(u => (u.UserRank == "Catmin")) : null;

            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "Username already exists");
                return View(registerVM);
            }
            else if (existingAdmin != null)
            {
                ModelState.AddModelError("UserName", "Admin has already exists");
                return View(registerVM);
            }

            var newUser = new User()
            {
                UserFullName = $"{registerVM.FirstName} {registerVM.LastName}",
                UserName = registerVM.UserName,
                UserPassword = registerVM.Password,
                UserRank = userRank
            };

            await _context.User.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {


            if (!ModelState.IsValid) return View(loginVM);

            var existingUser = await _context.User.FirstOrDefaultAsync(u => (u.UserName == loginVM.UserName));

            if (existingUser == null)
            {
                ModelState.AddModelError("UserName", "Username cannot found");
                return View(loginVM);
            }
            else if (existingUser.UserPassword != loginVM.Password)
            {
                ModelState.AddModelError("Password", "Incorrect password");
                return View(loginVM);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.UserId.ToString()),
                new Claim(CustomClaim.FullName, existingUser.UserFullName),
                new Claim(ClaimTypes.Name, existingUser.UserName),
                new Claim(CustomClaim.UserImgUrl, existingUser.UserImgUrl),
                new Claim(ClaimTypes.Role, existingUser.UserRank)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            
            return RedirectToAction("Index", "Home");

        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(PasswordVM passwordVM)
        {
            var loggedUser = await _context.User.FirstAsync(u => (u.UserId == passwordVM.UserId));
            
            if (passwordVM.currentPassword != loggedUser.UserPassword)
            {
                TempData["PasswordError"] = "Current password has entered wrong";
                TempData["ActiveTab"] = "Password";

                return RedirectToAction("Index", "Settings");
            }
            else if(passwordVM.currentPassword == passwordVM.newPassword)
            {
                TempData["PasswordError"] = "New password should be different";
                TempData["ActiveTab"] = "Password";

                return RedirectToAction("Index", "Settings");
            }
            else if(passwordVM.newPassword != passwordVM.confirmPassword)
            {
                TempData["PasswordError"] = "Passwords do not match";
                TempData["ActiveTab"] = "Password";

                return RedirectToAction("Index", "Settings");
            }
                loggedUser.UserPassword = passwordVM.newPassword;
                _context.User.Update(loggedUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Logout");
        }

        private string GetUserRank(string username)
        {
            if (username.EndsWith("@admin")) return "Catmin";

            else if (username.EndsWith("@moderator")) return "Moderadog";

            else return "Default";
        }
    }
}
