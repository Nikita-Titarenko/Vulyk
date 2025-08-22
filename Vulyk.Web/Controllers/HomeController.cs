using System.Diagnostics;
using Vulyk.Web.ViewModels;

namespace Vulyk.Web.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["CurrentPage"] = "Home";
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ComingSoon(string page)
        {
            ViewData["CurrentPage"] = page;
            return View();
        }

        [HttpGet]
        public IActionResult AboutUs()
        {
            ViewData["CurrentPage"] = "AboutUs";
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "AboutUs" });
        }

        [HttpGet]
        public IActionResult Contact()
        {
            ViewData["CurrentPage"] = "Contact";
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "Contact" });
        }

        [HttpGet]
        public IActionResult Services()
        {
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "Services" });
        }

        [HttpGet]
        public IActionResult Blog()
        {
            return RedirectToAction(nameof(ComingSoon), "Home", new { page = "Blog" });
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
