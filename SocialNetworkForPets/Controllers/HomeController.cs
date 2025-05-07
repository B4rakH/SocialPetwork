using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Home;
using SocialNetworkForPets.Helper;

namespace SocialNetworkForPets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        //Get the logged by UserId
        public int loggedInUserId = 1;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //Listing All Posts 
        public async Task<IActionResult> Index()
        {
            var allPosts = await _context.Post
                .Include(p => p.Poster)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

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


            //Checking The file Upload if exists
            if (post.Image != null && post.Image.Length > 0) 
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (post.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/uploaded");
                    Directory.CreateDirectory(rootFolderPathImages);
                    
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create)) 
                        await post.Image.CopyToAsync(stream);

                    newPost.PostImgUrl = "/images/uploaded/" + fileName;
                }

            }

            //Add to the database
            await _context.Post.AddAsync(newPost);
            await _context.SaveChangesAsync();

            //Finding and storing tags
            var postHashTags = HashtagHelper.GetHashtags(post.PostText);
            foreach (var tag in postHashTags) 
            {
                var hashtagDb = await _context.Hashtag.FirstOrDefaultAsync(t => t.TagText == tag);
                if(hashtagDb != null)
                {
                    hashtagDb.TagCount++;
                }
                else
                {
                    var newHashtag = new Hashtag()
                    {
                        TagText = tag,
                        TagCount = 1
                    };
                    await _context.Hashtag.AddAsync(newHashtag);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikes) 
        {
            var like = await _context.Like
                .Where(l => l.PostId == postLikes.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (like != null) 
            {
                _context.Like.Remove(like);
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postLikes.PostId,
                    UserId = loggedInUserId
                };
                
                await _context.Like.AddAsync(newLike);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavorites)
        {
            var favorite = await _context.Favorite
                .Where(l => l.PostId == postFavorites.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorite.Remove(favorite);
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postFavorites.PostId,
                    UserId = loggedInUserId
                };

                await _context.Favorite.AddAsync(newFavorite);
            }
            await _context.SaveChangesAsync();

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
            await _context.Comment.AddAsync(newComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment (RemoveCommentVM commentVM)
        {
            var commentDb = await _context.Comment
                .FirstOrDefaultAsync(c => c.CommentId == commentVM.CommentId);
            if (commentDb != null) 
            {
                _context.Comment.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemovePost(PostRemoveVM postVM)
        {
            var postDb = await _context.Post.FirstOrDefaultAsync(p => p.PostId == postVM.PostId);
            
            if (postDb != null)
            {
                //If post has comments, delete one by one first
                foreach(var comment in _context.Comment.Where(c => c.PostId == postDb.PostId)) _context.Comment.Remove(comment);
                
                _context.Post.Remove(postDb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

    }
}
