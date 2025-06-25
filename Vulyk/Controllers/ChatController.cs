using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Hubs;
using Vulyk.Models;
using Vulyk.Services;
using Vulyk.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using static Vulyk.Services.ChatService;

namespace Vulyk.Controllers
{
    public class ChatController : BaseController
    {
        private readonly ChatService _chatService;

        private readonly UserService _userService;

        private readonly IHubContext<ChatHub> _chatHub;
        public ChatController(UserService userService, ChatService chatService, IHubContext<ChatHub> chatHub)
        {
            _userService = userService;
            _chatService = chatService;
            _chatHub = chatHub;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? userToAddId, int? chatId)
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ChatListViewModel chatListViewModel = await GetChatListViewModel(userId.Value);

            chatListViewModel.NewUserId = userToAddId;
            chatListViewModel.DisplayChatId = chatId;
            chatListViewModel.UserId = userId.Value;

            return View(chatListViewModel);
        }

        public async Task<ChatListViewModel> GetChatListViewModel(int userId)
        {
            return new ChatListViewModel
            {
                ChatItemsViewModels = (await _chatService.GetChatsAsync(userId))
    .Select(c => new ChatListItemViewModel
    {
        ChatId = c.ChatId,
        UserId = c.UserId,
        LastMessageText = c.LastMessageText,
        LastMessageDateTime = c.LastMessageDateTime,
        Name = c.Name
    }).ToList()
            };
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
            int? userToAddId = await _userService.FindUserAsync(createChatViewModel.LoginToAdd!, createChatViewModel.PhoneToAdd!, createChatViewModel.CreateType);
            if (userToAddId == null)
            {
                ModelState.AddModelError(string.Empty, $"User with this {(createChatViewModel.CreateType.Equals(CreateType.Login) ? "login" : "phone")} not exist");
                return View(createChatViewModel);
            }
            int? chatId = await _chatService.GetChatAsync(userId.Value, userToAddId.Value);
            if (chatId == null)
            {
                return RedirectToAction("Index", "Chat", new { userToAddId });
            }

            return RedirectToAction("Index", "Chat", new { chatId });
        }
    }
}
