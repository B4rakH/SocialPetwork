using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;

namespace SocialNetworkForPets.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string txtsearch)
        {
            //Searching user profile based on username input
            var User = await _context.User.FirstOrDefaultAsync(u => u.UserName == txtsearch);
            
            //Return homepage if cannot found
            if (User == null) return RedirectToAction("Index", "Home");

            //Open Profile of the user
            return RedirectToAction("Details", "Users", new { userId = User.UserId });

        }
    }
}
