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
            var User = await _context.User.FirstOrDefaultAsync(u => u.UserName == txtsearch);
            
            if (User == null) return RedirectToAction("Index", "Home");

            return RedirectToAction("Details", "Users", new { userId = User.UserId });


        }
    }
}
