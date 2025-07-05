using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.Models;

namespace Vulyk.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (GetUserIdFromCookie() == null)
            {
                ViewData["CurrentPage"] = "Home";
                return View();
            }
            return RedirectToAction("Index", "Chat");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ComingSoon(string page)
        {
            ViewData["CurrentPage"] = page;
            return View();
        }

        public IActionResult AboutUs()
        {
            ViewData["CurrentPage"] = "AboutUs";
            return RedirectToAction("ComingSoon", "Home", new { page = "AboutUs" });
        }

        public IActionResult Contact()
        {
            ViewData["CurrentPage"] = "Contact";
            return RedirectToAction("ComingSoon", "Home", new { page = "Contact" });
        }

        public IActionResult Services()
        {
            return RedirectToAction("ComingSoon", "Home", new { page = "Services" });
        }

        public IActionResult Blog()
        {
            return RedirectToAction("ComingSoon", "Home", new { page = "Blog" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
