using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Hubs;
using Vulyk.Entities;
using Vulyk.Services;
using Vulyk.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using static Vulyk.Services.ChatService;

namespace Vulyk.Controllers
{
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;

        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        public ChatController(IChatService chatService, IMapper mapper, IUserService userService)
        {
            _chatService = chatService;
            _mapper = mapper;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(string? userToAddId, int? chatId)
        {
            ViewData["ChoosedPage"] = "Chats";
            string userId = GetUserId()!;

            ChatListViewModel chatListViewModel = await GetChatListViewModel(userId);

            chatListViewModel.NewUserId = userToAddId;
            chatListViewModel.DisplayChatId = chatId;
            chatListViewModel.UserId = userId;
            string? userName = await _userService.GetFullNameAsync(chatListViewModel.UserId);
            if (userName != null)
            {
                chatListViewModel.FullName = userName;
            }
            ViewData["SidepanelVisibility"] = true;
            return View(chatListViewModel);
        }

        public async Task<ChatListViewModel> GetChatListViewModel(string userId)
        {
            return _mapper.Map<ChatListViewModel>(await _chatService.GetChatsAsync(userId));
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ChoosedPage"] = "CreateChat";
            ViewData["SidepanelVisibility"] = false;

            return View(new LoginViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmailInputViewModel emailInputChatViewModel)
        {
            ViewData["ChoosedPage"] = "CreateChat";
            ViewData["SidepanelVisibility"] = false;
            if (!ModelState.IsValid)
            {
                return View(emailInputChatViewModel);
            }

            string? userId = GetUserId()!;

            var (foundUserId, findUserResult) = await _userService.FindUserByEmailAsync(emailInputChatViewModel.Email);
            if (findUserResult == UserService.FindUserResult.LoginFailed)
            {
                ModelState.AddModelError(string.Empty, $"User with this email not exist");
                return View(emailInputChatViewModel);
            }
            if (userId == foundUserId)
            {
                ModelState.AddModelError(string.Empty, "You don't can add yourself");
                return View(emailInputChatViewModel);
            }
            int? chatId = await _chatService.GetChatAsync(userId, foundUserId!);

            return RedirectToAction(nameof(Index), "Chat", new { userToAddId = foundUserId, chatId });
        }
    }
}
