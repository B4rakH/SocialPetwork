using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Helper.Constants;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SocialNetworkForPets.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected int? GetUserId()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return (UserId != null) ? int.Parse(UserId) : null;
            
        }

        protected IActionResult RedirectToLogin()
        {

            //In case of bug, safe logout can be implemented
            return RedirectToAction("Login", "Authentication");
        }
    }
}
