using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetworkForPets.Data;
using SocialNetworkForPets.Services;

namespace SocialNetworkForPets.ViewComponents
{
    public class HashtagViewComponent: ViewComponent
    {
        private readonly IHashtagService _hashtagService;

        public HashtagViewComponent(IHashtagService hashtagService)
        {
            _hashtagService = hashtagService;
        }
        public async Task <IViewComponentResult> InvokeAsync()
        {
            var trendTopics = await _hashtagService.GetTrendTopics();
            return View(trendTopics);
        }
    }
}
