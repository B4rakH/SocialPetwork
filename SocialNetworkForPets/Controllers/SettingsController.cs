using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Helper.Enums;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.ViewModels.Settings;

namespace SocialNetworkForPets.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IFileService _fileService;
        int UserId = 1;
        public SettingsController(IUsersService usersService, IFileService fileService)
        {
            _usersService = usersService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var userDb = await _usersService.GetUserAsync(UserId);
            
            return View(userDb);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(ProfilePictureVM profilePictureVM)
        {
            var profilePicture = await _fileService.UploadImageAsync
                (profilePictureVM.profilePicture, ImageFileType.ProfilePicture);
            await _usersService.UpdateProfilePicture(UserId, profilePicture);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileVM profileVM)
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(PasswordVM passwordVM)
        {
            return RedirectToAction("Index");
        }
    }
}
