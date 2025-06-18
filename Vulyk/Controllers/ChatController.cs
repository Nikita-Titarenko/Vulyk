using Microsoft.AspNetCore.Mvc;
using Vulyk.Data;
using Vulyk.Models;
using Vulyk.Services;
using static Vulyk.Services.ChatService;

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
            if (!ModelState.IsValid)
            {
                return View(createChatViewModel);
            }
            ChatService chatService = new ChatService(_context);
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            try
            {
                (CreateChatResult, int?) createChutResult = await chatService.CreateChatAsync(userId.Value, createChatViewModel.LoginToAdd, createChatViewModel.PhoneToAdd, createChatViewModel.CreateType);
                if (createChutResult.Item1.Equals(CreateChatResult.NotFound))
                {
                    ModelState.AddModelError(string.Empty, $"User with this {(createChatViewModel.CreateType.Equals(CreateType.Login) ? "login" : "phone")} not exist");
                    return View(createChatViewModel);
                }
                if (createChutResult.Item1.Equals(CreateChatResult.CanNotAddYourself))
                {
                    ModelState.AddModelError(string.Empty, $"It's your {(createChatViewModel.CreateType.Equals(CreateType.Login) ? "login" : "phone")}");
                    return View(createChatViewModel);
                }
                return RedirectToAction("Index", "Chat");
            }
            catch
            {
                return ShowUnexpectedError();
            }
        }
    }
}
