using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Helper.Constants;
using SocialNetworkForPets.Helper.Enums;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.ViewModels.Settings;
using System.Security.Claims;
using SocialNetworkForPets.Controllers.Base;

namespace SocialNetworkForPets.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly IUsersService _usersService;
        private readonly IFileService _fileService;
        public SettingsController(IUsersService usersService, IFileService fileService)
        {
            _usersService = usersService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var userDb = await _usersService.GetUserAsync(UserId.Value);
            
            return View(userDb);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(ProfilePictureVM profilePictureVM)
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var profilePicture = await _fileService.UploadImageAsync
                (profilePictureVM.profilePicture, ImageFileType.ProfilePicture);
                    await _usersService.UpdateProfilePicture(UserId.Value, profilePicture);

            var cookiesUpdated = await UpdateCookiesAsync();

            if(!cookiesUpdated) return RedirectToLogin();

            return RedirectToAction("Index");
        }

        private async Task<bool> UpdateCookiesAsync()
        {
            var userId = GetUserId();
            if (userId == null) return false;

            var user = await _usersService.GetUserAsync(userId.Value);

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
