using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly IPostService _postService;
        int loggedInUser = 1;

        public FavoriteController(IPostService postService)
        {
            _postService = postService;
        }
        public async Task<IActionResult> Index()
        {
            var myFavoritePosts = await _postService.GetAllFavoritePostsAsync(loggedInUser);

            return View(myFavoritePosts);
        }
    }
}
