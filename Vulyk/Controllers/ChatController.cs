using AutoMapper;
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

        private readonly IMapper _mapper;

        public ChatController(UserService userService, ChatService chatService, IMapper mapper)
        {
            _userService = userService;
            _chatService = chatService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? userToAddId, int? chatId)
        {
            ViewData["ChoosedPage"] = "Chats";
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
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
            return _mapper.Map<ChatListViewModel>(await _chatService.GetChatsAsync(userId));
        }

        public IActionResult Create()
        {
            ViewData["ChoosedPage"] = "CreateChat";
            ViewData["SidepanelVisibility"] = false;
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(new EmailInputViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmailInputViewModel createChatViewModel)
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
                return RedirectToAction(nameof(HomeController.Index), "Home");
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

            return RedirectToAction(nameof(Index), "Chat", new { userToAddId = foundUserId, chatId });
        }
    }
}
