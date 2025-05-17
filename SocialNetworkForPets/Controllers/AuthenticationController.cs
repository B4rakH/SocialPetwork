using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using SocialNetworkForPets.Helper.Constants;
using SocialNetworkForPets.ViewModels.Settings;
using System.Text.RegularExpressions;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.ViewModels.Home;

namespace SocialNetworkForPets.Controllers
{
    public class AuthenticationController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IPostService _postService;
        public AuthenticationController(AppDbContext context, IPostService postService)
        {
            _context = context;
            _postService = postService;
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

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(int userId, string confirmCurrentPassword)
        {
            var user = await _context.User.FirstAsync(u => u.UserId == userId);

            if(user.UserPassword != confirmCurrentPassword) 
            {
                TempData["DeleteConfirmError"] = "Current password has entered wrong";
                TempData["ActiveTab"] = "Profile";

                return RedirectToAction("Index", "Settings");
            }

            await DeleteAccountHelper(userId, user);

            return await Logout();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccountAdmin(int userId)
        {
            var user = await _context.User.FirstAsync(u => u.UserId == userId);

            await DeleteAccountHelper(userId, user);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        private async Task DeleteAccountHelper(int userId, User user)
        {

            foreach (var like in _context.Like.Where(l => l.UserId == userId).ToList()) _context.Like.Remove(like);

            foreach (var comment in _context.Comment.Where(c => c.UserId == userId).ToList()) _context.Comment.Remove(comment);

            foreach (var favorite in _context.Favorite.Where(f => f.UserId == userId).ToList()) _context.Favorite.Remove(favorite);

            foreach (var report in _context.Report.Where(r => r.UserId == userId).ToList()) _context.Report.Remove(report);

            foreach (var post in _context.Post.Where(p => p.PosterId == userId).ToList()) await _postService.RemovePostAsync(post.PostId);

            foreach (var notification in _context.Notification.Where(n => n.UserId == userId).ToList()) _context.Notification.Remove(notification);

            foreach (var friendship in _context.Friendship.Where(f => (f.User1Id == userId) || (f.User2Id == userId)).ToList())
                _context.Friendship.Remove(friendship);

            foreach (var request in _context.FriendRequests.Where(f => (f.SenderId == userId) || (f.ReceiverId == userId)).ToList())
                _context.FriendRequests.Remove(request);


            _context.User.Remove(user);

            await _context.SaveChangesAsync();
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
