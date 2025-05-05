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

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //Listing All Posts 
        public async Task<IActionResult> Index()
        {
            var allPosts = await _context.Post
                .Include(u => u.Poster)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(allPosts);
        }        


        [HttpPost]

        //Creating New Post
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            int loogedInUser = 1;

            //Setting variables
            var newPost = new Post
            {
                PostText = post.PostText,
                CreatedAt = DateTime.Now,
                PosterId = loogedInUser,
            };

            //Checking The file Upload if exists
            if (post.Image != null && post.Image.Length > 0) 
            {
                var rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot");

                if (post.Image.ContentType.Contains("image"))
                {
                    var rootFolderPathImages = Path.Combine(rootFolderPath, "images");
                    Directory.CreateDirectory(rootFolderPathImages);
                    
                    var imgFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(post.Image.FileName)}";
                    var imgFilePath = Path.Combine(rootFolderPathImages, imgFileName);

                    using (var stream = new FileStream(imgFilePath, FileMode.Create)) 
                        await post.Image.CopyToAsync(stream);

                    //Set the URL to the newPost object
                    newPost.PostImgUrl = $"/images/{imgFilePath}";
                    
                }

            }


            await _context.Post.AddAsync(newPost);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
