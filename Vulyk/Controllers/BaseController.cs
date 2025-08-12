using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Vulyk.Controllers
{
    public class BaseController : Controller
    {
        public string? GetUserId()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return null;
            }
            return userId;
        }
    }
}
