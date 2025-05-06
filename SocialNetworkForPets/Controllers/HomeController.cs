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
            //Get the logged by UserId
            int loggedInUser = 8;

            //Setting variables
            var newPost = new Post
            {
                PostText = post.PostText,
                CreatedAt = DateTime.Now,
                PosterId = loggedInUser,
            };


            //TODO: Dosyayý images klasörüne atýp yolunu çýkararak PostImgUrl deðerine ata ve <img> ile açýlmasýný saðla
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
    }
}
