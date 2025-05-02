using Microsoft.AspNetCore.Mvc;
using SocialNetworkForPets.Models;
using System.Diagnostics;

namespace SocialNetworkForPets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
