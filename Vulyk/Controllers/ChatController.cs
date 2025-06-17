using Microsoft.AspNetCore.Mvc;
using Vulyk.Data;
using Vulyk.Models;
using Vulyk.Services;

namespace Vulyk.Controllers
{
    public class ChatController : BaseController
    {
        private ApplicationDbContext _context;
        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            UserService userService = new UserService(_context);
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            User? user = await userService.FindUserAsync(userId.Value);
            if (user == null)
            {
                return ShowUnexpectedError();
            }
            CreateChatViewModel createChatViewModel = new CreateChatViewModel
            {
                Login = user.Login
            };
            return View(createChatViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateChatViewModel createChatViewModel)
        {
            return RedirectToAction("Index", "Chat");
        }
    }
}
