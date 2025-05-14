using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.Controllers
{
    [Authorize]
    public class FavoriteController : BaseController
    {
        private readonly IPostService _postService;


        public FavoriteController(IPostService postService)
        {
            _postService = postService;
        }
        public async Task<IActionResult> Index()
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var myFavoritePosts = await _postService.GetAllFavoritePostsAsync(UserId.Value);

            return View(myFavoritePosts);
        }
    }
}
