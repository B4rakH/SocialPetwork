using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Helper.Constants;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.Controllers
{
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
        public async Task<IActionResult> ApproveReportAsync(int postId)
        {
            await _moderatorService.ApproveReport(postId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectReportAsync(int postId)
        {
            await _moderatorService.RejectReport(postId);

            return RedirectToAction("Index");
        }
    }
}
