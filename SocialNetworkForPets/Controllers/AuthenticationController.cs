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
using System.Text.RegularExpressions;
using SocialNetworkForPets.Controllers.Base;

namespace SocialNetworkForPets.Controllers
{
    public class AuthenticationController : BaseController
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

            var existingAdmin = (userRank == UserRank.Admin) ?
                await _context.User.FirstOrDefaultAsync(u => (u.UserRank == UserRank.Admin)) : null;

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
        public async Task<IActionResult> UpdatePassword(UpdatePasswordVM passwordVM)
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
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileVM profileVM) 
        {
            var validErrorCheck = await UpdateProfileHelper(profileVM);

            if(validErrorCheck != null)
            {
                TempData["UpdateError"] = validErrorCheck;
                TempData["ActiveTab"] = "Profile";

                return RedirectToAction("Index", "Settings");
            }

            var loggedUser = await _context.User.FirstAsync(u => u.UserId == profileVM.UserId);

            if ((loggedUser.UserRank == UserRank.Admin && !profileVM.UserName.Contains("@admin"))
                ||(loggedUser.UserRank == UserRank.Moderator && profileVM.UserName.Contains("@moderator")))
            {
                TempData["UpdateError"] = "Username must contain the tag of user rank (@rank)";
                TempData["ActiveTab"] = "Profile";

                return RedirectToAction("Index", "Settings");
            }

            loggedUser.UserFullName = profileVM.UserFullName;
            loggedUser.UserName = profileVM.UserName;

            _context.User.Update(loggedUser);
            await _context.SaveChangesAsync();

            var cookiesUpdated = await UpdateCookiesAsync();

            if (!cookiesUpdated) return RedirectToLogin();
            

            TempData["UpdateSuccess"] = "Informations Updated successfully";
            TempData["ActiveTab"] = "Profile";

            return RedirectToAction("Index", "Settings");
        }

        private string GetUserRank(string username)
        {
            if (username.EndsWith("@admin")) return UserRank.Admin;

            else if (username.EndsWith("@moderator")) return UserRank.Moderator;

            else return UserRank.Default;
        }

        private async Task<string> UpdateProfileHelper(UpdateProfileVM profileVM)
        {
            if (profileVM.UserFullName == null || profileVM.UserName == null)
            {
                return "Please fill the required areas";
            }

            else if (profileVM.UserFullName.Count(c => c != ' ') < 3 || profileVM.UserFullName.Length > 100)
            {
                return "Full name length must be between 3-100 characters";

            }
            else if (!Regex.IsMatch(profileVM.UserFullName, @"^[a-zA-Z\s]+$"))
            {
                return "Full name must contain letters and white-spaces only";

            }
            else if (profileVM.UserName.Length < 2 || profileVM.UserName.Length > 50)
            {
                return "Username must be between 2-50 characters";
            }
            else if (!Regex.IsMatch(profileVM.UserName, @"^[a-z0-9@._\-]+$"))
            {
                return "Username must contain only numbers, lowercase letters and ('@' '.' '_' '-') symbols";
            }
            var isUserNameExist = await _context.User.CountAsync(u => u.UserName == profileVM.UserName);

            if (isUserNameExist > 1)
            {
                return "The username is already exists";
            }
            return null;
        }

        private async Task<bool> UpdateCookiesAsync()
        {
            var userId = GetUserId();
            if (userId == null) return false;

            var user = await _context.User.FindAsync(userId);

            var newClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(CustomClaim.FullName, user.UserFullName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(CustomClaim.UserImgUrl, user.UserImgUrl),
                new Claim(ClaimTypes.Role, user.UserRank)
            };

            var identity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return true;
        }
    }
}
