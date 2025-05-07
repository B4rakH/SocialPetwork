using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Home;
using SocialNetworkForPets.Helper;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IPostService _postService;
        private readonly IHashtagService _hashtagService;

        //Get the logged by UserId
        public int loggedInUserId = 1;

        public HomeController
            (ILogger<HomeController> logger,
                AppDbContext context,
                    IPostService postService,
                        IHashtagService hashtagService)
        {
            _logger = logger;
            _context = context;
            _postService = postService;
            _hashtagService = hashtagService;
        }

        //Listing All Posts 
        public async Task<IActionResult> Index()
        {
            var allPosts = await _postService.GetAllPostsAsync(loggedInUserId);

            return View(allPosts);
        }        


        [HttpPost]

        //Creating New Post
        public async Task<IActionResult> CreatePost(PostVM post)
        {

            //Setting variables
            var newPost = new Post
            {
                PostText = post.PostText,
                CreatedAt = DateTime.Now,
                PosterId = loggedInUserId,
            };

            await _postService.CreatePostAsync(newPost, post.Image);
            await _hashtagService.HashtagsInNewPostAsync(post.PostText);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikes) 
        {
            await _postService.TogglePostLikeAsync(postLikes.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavorites)
        {
            await _postService.TogglePostFavoriteAsync(postFavorites.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task <IActionResult> AddComment (CommentVM commentVM)
        {
            var newComment = new Comment()
            {
                PostId = commentVM.PostId,
                UserId = loggedInUserId,
                CommentText = commentVM.CommentText
            };
            await _postService.AddPostCommentAsync(newComment);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment (RemoveCommentVM commentVM)
        {
            await _postService.RemovePostCommentAsync(commentVM.CommentId);
            return RedirectToAction("Index");
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
