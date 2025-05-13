using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Helper.Constants;
using System.Security.Claims;

namespace SocialNetworkForPets.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected int? GetUserId()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return (UserId != null) ? int.Parse(UserId) : null;
            
        }

        protected string? GetUserFullName()
        {
            var fullname = User.FindFirstValue(CustomClaim.FullName);
            
            return fullname;
        }

        protected IActionResult RedirectToLogin()
        {

            //In case of bug, safe logout can be implemented
            return RedirectToAction("Login", "Authentication");
        }
    }
}
