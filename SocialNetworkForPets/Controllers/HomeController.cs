using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Data.Models;
using SocialNetworkForPets.ViewModels.Home;

namespace SocialNetworkForPets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        //Get the logged by UserId
        public int loggedInUser = 1;

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
                PosterId = loggedInUser,
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

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikes) 
        {
            var like = await _context.Like
                .Where(l => l.PostId == postLikes.PostId && l.UserId == loggedInUser)
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
                    UserId = loggedInUser
                };
                
                await _context.Like.AddAsync(newLike);
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
                UserId = loggedInUser,
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
    }
}
