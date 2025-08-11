using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.Entities;

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
            ViewData["CurrentPage"] = "Home";
            return View();
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
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "AboutUs" });
        }

        public IActionResult Contact()
        {
            ViewData["CurrentPage"] = "Contact";
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "Contact" });
        }

        public IActionResult Services()
        {
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "Services" });
        }

        public IActionResult Blog()
        {
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "Blog" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
