using Microsoft.AspNetCore.Mvc;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.Services;
using Vulyk.ViewModels;
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
        public async Task<IActionResult> Index()
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ChatService chatService = new ChatService(_context);
            ChatListViewModel chatPageViewModel = new ChatListViewModel
            {
                chatItemViewModels = (await chatService.GetChatsAsync(userId.Value))
                .Select(c => new ChatListItemViewModel
                {
                    LastMessageText = c.LastMessageText,
                    LastMessageDateTime = c.LastMessageDateTime,
                    ChatId = c.ChatId,
                    Name = c.Name
                }).ToList()
            };

            return View(chatPageViewModel);
        }

        public async Task<IActionResult> Create()
        {
            UserService userService = new UserService(_context);
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            string? login = await userService.GetUserLoginAsync(userId.Value);
            if (login == null)
            {
                return ShowUnexpectedError();
            }
            CreateChatViewModel createChatViewModel = new CreateChatViewModel
            {
                Login = login
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
