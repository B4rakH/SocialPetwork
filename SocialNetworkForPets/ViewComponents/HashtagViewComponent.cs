using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;

namespace SocialNetworkForPets.ViewComponents
{
    public class HashtagViewComponent: ViewComponent
    {

        private readonly AppDbContext _context;

        public HashtagViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task <IViewComponentResult> InvokeAsync()
        {
            var topHashtags = await _context.Hashtag
                .OrderByDescending(tag => tag.TagCount)
                .Take(3)
                .ToListAsync();

            return View(topHashtags);
        }
    }
}
