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

        public ChatController(UserService userService, ChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? userToAddId, int? chatId)
        {
            ViewData["ChoosedPage"] = "Chats";
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ChatListViewModel chatListViewModel = await GetChatListViewModel(userId.Value);

            chatListViewModel.NewUserId = userToAddId;
            chatListViewModel.DisplayChatId = chatId;
            chatListViewModel.UserId = userId.Value;
            string? userName = await _userService.GetUserNameAsync(chatListViewModel.UserId);
            if (userName != null)
            {
                chatListViewModel.FullName = userName;
            }
            ViewData["SidepanelVisibility"] = true;
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

        public IActionResult Create()
        {
            ViewData["ChoosedPage"] = "CreateChat";
            ViewData["SidepanelVisibility"] = false;
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new CreateChatViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateChatViewModel createChatViewModel)
        {
            ViewData["ChoosedPage"] = "CreateChat";
            ViewData["SidepanelVisibility"] = false;
            if (!ModelState.IsValid)
            {
                return View(createChatViewModel);
            }

            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var (foundUserId, findUserResult) = await _userService.FindUserAsync(createChatViewModel.Email);
            if (findUserResult != UserService.FindUserResult.Registered)
            {
                ModelState.AddModelError(string.Empty, $"User with this email not exist");
                return View(createChatViewModel);
            }
            if (userId == foundUserId)
            {
                ModelState.AddModelError(string.Empty, "You don't can add yourself");
                return View(createChatViewModel);
            }
            int? chatId = await _chatService.GetChatAsync(userId.Value, foundUserId!.Value);

            return RedirectToAction("Index", "Chat", new { userToAddId = foundUserId, chatId });
        }
    }
}
