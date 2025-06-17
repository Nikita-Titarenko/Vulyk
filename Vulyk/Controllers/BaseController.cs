using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Vulyk.Controllers
{
    public class BaseController : Controller
    {
        public int? GetUserIdFromCookie()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return null;
            }
            return int.Parse(userId);
        }

        public IActionResult ShowUnexpectedError()
        {
            ModelState.AddModelError(string.Empty, "Occur unexpected error!");
            return View();
        }
    }
}
