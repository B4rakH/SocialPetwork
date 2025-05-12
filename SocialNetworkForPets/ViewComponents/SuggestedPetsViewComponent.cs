using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Services;
using System.Security.Claims;

namespace SocialNetworkForPets.ViewComponents
{
    public class SuggestedPetsViewComponent: ViewComponent
    {
        private readonly IFriendsService _friendsService;

        public SuggestedPetsViewComponent(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
            var UserId = int.Parse(loggedUserId);

            var suggestedPets = await _friendsService.GetSuggestedPetsAsync(UserId);


            return View(suggestedPets);
        }
    }
}
