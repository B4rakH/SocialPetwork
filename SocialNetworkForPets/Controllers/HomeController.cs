using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Home;
using SocialNetworkForPets.Services;
using SocialNetworkForPets.Helper.Enums;
using Microsoft.AspNetCore.Authorization;
using SocialNetworkForPets.Controllers.Base;
using SocialNetworkForPets.Helper.Constants;
using Microsoft.EntityFrameworkCore;

namespace SocialNetworkForPets.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IPostService _postService;
        private readonly IFileService _fileService;
        private readonly INotificationService _notificationService;

        //HomeController is the main controller that contains and uses all services
        public HomeController
            (ILogger<HomeController> logger,
                AppDbContext context,
                    IPostService postService,
                        IHashtagService hashtagService,
                            IFileService fileService,
                                INotificationService notificationService)
        {
            _logger = logger;
            _context = context;
            _postService = postService;
            _fileService = fileService;
            _notificationService = notificationService;
        }

        //Listing All Posts 
        public async Task<IActionResult> Index()
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();
            
            //Collecting all posts for homepage
            var allPosts = await _postService.GetAllPostsAsync(UserId.Value);

            return View(allPosts);
        }  
        
        public async Task<IActionResult> Details(int postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);

            return View(post);
        }


        [HttpPost]

        //Creating New Post
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            //Storing image on uploaded folder (if it exists)
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

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikes) 
        {
            var UserId = GetUserId();
            var fullname = GetUserFullName();
            if (UserId == null) return RedirectToLogin();

            var result = await _postService.TogglePostLikeAsync(postLikes.PostId, UserId.Value);

            var post = await _postService.GetPostByIdAsync(postLikes.PostId);
            
            //If like stored successfully and post is not user's post, send notification to the poster
            if (result.SendNotification && post.PosterId != UserId.Value)
                await _notificationService.AddNewNotificationAsync
                    (post.PosterId, NotificationType.Like, fullname, postLikes.PostId);


            return PartialView("Home/_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavorites)
        {
            var UserId = GetUserId();
            var fullname = GetUserFullName();
            if (UserId == null) return RedirectToLogin();

            var result = await _postService.TogglePostFavoriteAsync(postFavorites.PostId, UserId.Value);

            var post = await _postService.GetPostByIdAsync(postFavorites.PostId);

            //If favorite stored successfully and post is not user's post, send notification to the poster
            if (result.SendNotification && post.PosterId != UserId.Value)
                await _notificationService.AddNewNotificationAsync
                    (post.PosterId, NotificationType.Favorite, fullname, postFavorites.PostId);

            return PartialView("Home/_Post", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> AddComment (CommentVM commentVM)
        {
            var UserId = GetUserId();
            var fullname = GetUserFullName();
            if (UserId == null) return RedirectToLogin();

            var newComment = new Comment()
            {
                PostId = commentVM.PostId,
                UserId = UserId.Value,
                CommentText = commentVM.CommentText
            };

            await _postService.AddPostCommentAsync(newComment);

            var post = await _postService.GetPostByIdAsync(commentVM.PostId);

            //sending new comment notification to the poster
            if(UserId != post.PosterId)
            await _notificationService.AddNewNotificationAsync
                    (post.PosterId, NotificationType.Comment, fullname, commentVM.PostId);

            return PartialView("Home/_Post", post);
        }
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var UserId = GetUserId();
            if (UserId == null) return RedirectToLogin();

            //Checking if its already reported, return without create again
            var isReported = await _context.Report
                .AnyAsync(r => r.UserId == UserId && r.PostId == postReportVM.PostId);

            if (!isReported)
            {
                var newReport = new Report()
                {
                    PostId = postReportVM.PostId,
                    UserId = UserId.Value
                };
                await _postService.AddPostReportAsync(newReport);
            }
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
            await _postService.RemovePostAsync(postVM.PostId);

            return RedirectToAction("Index");
        }
    }
}
