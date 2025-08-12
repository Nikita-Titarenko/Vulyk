using System.Security.Claims;
using AutoMapper;
using Vulyk.Services;
using Vulyk.ViewModels;

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
            var getChatsResult = await _chatService.GetChatsAsync(userId);

            if (!getChatsResult.IsSuccess)
            {
                foreach (var error in getChatsResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return View();
            }
            ChatListViewModel chatListViewModel = _mapper.Map<ChatListViewModel>(getChatsResult.Value);

            chatListViewModel.NewUserId = userToAddId;
            chatListViewModel.DisplayChatId = chatId;
            chatListViewModel.UserId = userId;

            var getFullNameResult = await _userService.GetFullNameAsync(chatListViewModel.UserId);
            if (!getFullNameResult.IsSuccess)
            {
                foreach (var error in getFullNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return View();
            }
            chatListViewModel.FullName = getFullNameResult.Value.FullName;

            ViewData["SidepanelVisibility"] = true;
            return View(chatListViewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ChoosedPage"] = "CreateChat";
            ViewData["SidepanelVisibility"] = false;

            return View();
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

            if (User.FindFirstValue(ClaimTypes.Email) == emailInputChatViewModel.Email)
            {
                ModelState.AddModelError(string.Empty, "You don't can add yourself");
                return View(emailInputChatViewModel);
            }

            string userId = GetUserId()!;

            var getUserChatResult = await _chatService.GetUserChatByEmailAsync(userId, emailInputChatViewModel.Email);

            if (!getUserChatResult.IsSuccess)
            {
                foreach (var error in getUserChatResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return View();
            }

            if (getUserChatResult.Value.ChatId != null)
            {
                return RedirectToAction(nameof(Index), "Chat", new { chatId = getUserChatResult.Value.ChatId, userToAddId = getUserChatResult.Value.UserId });
            }

            return RedirectToAction(nameof(Index), "Chat", new { userToAddId = getUserChatResult.Value.UserId });
        }
    }
}
