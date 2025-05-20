using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Helper.Constants;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.Controllers
{
    //Annotation for make page only accessible for Admin and Moderator
    [Authorize(Roles = $"{UserRank.Moderator},{UserRank.Admin}")]
    public class ModeratorController : BaseController
    {
        private readonly IModeratorService _moderatorService;

        public ModeratorController(IModeratorService moderatorService)
        {
            _moderatorService = moderatorService;
        }
        public async Task<IActionResult> Index()
        {
            var reportedPosts = await _moderatorService.GetReportedPostsAsync();

            return View(reportedPosts);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveReport(int postId)
        {
            //Deleting Post
            await _moderatorService.ApproveReport(postId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectReport(int postId)
        {
            //Cleaning reports without deleting post
            await _moderatorService.RejectReport(postId);

            return RedirectToAction("Index");
        }
    }
}
