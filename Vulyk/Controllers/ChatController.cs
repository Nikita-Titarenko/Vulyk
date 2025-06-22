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
        private readonly ChatService _chatService;

        private readonly UserService _userService;
        public ChatController(UserService userService, ChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;
        }
        public async Task<IActionResult> Index()
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ChatListViewModel chatPageViewModel = new ChatListViewModel
            {
                chatItemViewModels = (await _chatService.GetChatsAsync(userId.Value))
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
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            string? login = await _userService.GetUserLoginAsync(userId.Value);
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

            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            try
            {
                (CreateChatResult, int?) createChutResult = await _chatService.CreateChatAsync(userId.Value, createChatViewModel.LoginToAdd!, createChatViewModel.PhoneToAdd!, createChatViewModel.CreateType);
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
