using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Home;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.Helper.Enums;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkForPets.Controllers.Base;

namespace SocialNetworkForPets.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IPostService _postService;
        private readonly IHashtagService _hashtagService;
        private readonly IFileService _fileService;

        public HomeController
            (ILogger<HomeController> logger,
                AppDbContext context,
                    IPostService postService,
                        IHashtagService hashtagService,
                            IFileService fileService)
        {
            _logger = logger;
            _context = context;
            _postService = postService;
            _hashtagService = hashtagService;
            _fileService = fileService;
        }

        //Listing All Posts 
        public async Task<IActionResult> Index()
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();
            
            var allPosts = await _postService.GetAllPostsAsync(UserId.Value);

            return View(allPosts);
        }        


        [HttpPost]

        //Creating New Post
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var imageUploadPath = await _fileService.UploadImageAsync(post.Image, ImageFileType.PostImage);
            //Setting variables
            var newPost = new Post
            {
                PostText = post.PostText,
                CreatedAt = DateTime.Now,
                PosterId = UserId.Value,
                PostImgUrl = imageUploadPath
            };

            await _postService.CreatePostAsync(newPost);
            await _hashtagService.HashtagsInNewPostAsync(post.PostText);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikes) 
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            await _postService.TogglePostLikeAsync(postLikes.PostId, UserId.Value);

            var post = await _postService.GetPostByIdAsync(postLikes.PostId);

            return PartialView("Home/_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavorites)
        {

            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            await _postService.TogglePostFavoriteAsync(postFavorites.PostId, UserId.Value);

            var post = await _postService.GetPostByIdAsync(postFavorites.PostId);

            return PartialView("Home/_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> AddComment (CommentVM commentVM)
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var newComment = new Comment()
            {
                PostId = commentVM.PostId,
                UserId = UserId.Value,
                CommentText = commentVM.CommentText
            };
            await _postService.AddPostCommentAsync(newComment);

            var post = await _postService.GetPostByIdAsync(commentVM.PostId);

            return PartialView("Home/_Post", post);
        }
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            var newReport = new Report()
            {
                PostId = postReportVM.PostId,
                UserId = UserId.Value
            };
            await _postService.AddPostReportAsync(newReport);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment (RemoveCommentVM commentVM)
        {
            await _postService.RemovePostCommentAsync(commentVM.CommentId);

            var post = await _postService.GetPostByIdAsync(commentVM.PostId);

            return PartialView("Home/_Post", post);
        }
        [HttpPost]
        public async Task<IActionResult> RemovePost(PostRemoveVM postVM)
        {
            var deletedPost = await _postService.RemovePostAsync(postVM.PostId);
            await _hashtagService.HashtagsInRemovedPostAsync(deletedPost.PostText);
            return RedirectToAction("Index");
        }

    }
}
