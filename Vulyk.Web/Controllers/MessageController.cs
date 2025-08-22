using AutoMapper;
using Vulyk.Application.DTOs.Message;
using Vulyk.Application.Services.Message;
using Vulyk.Application.Services.User;
using Vulyk.Web.ViewModels.Message;

namespace Vulyk.Web.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;

        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        public MessageController(IMessageService messageService, IUserService userService, IMapper mapper)
        {
            _messageService = messageService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(int chatId, string partnerId)
        {
            string userId = GetUserId()!;
            var dto = new GetMessagesDto { ChatId = chatId, UserId = userId, PartnerId = partnerId };
            var getMessagesResult = await _messageService.GetMessagesAsync(dto);
            if (!getMessagesResult.IsSuccess)
            {
                foreach (var error in getMessagesResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return View();
            }

            var messageListViewModel = _mapper.Map<MessageListViewModel>(getMessagesResult.Value);
            messageListViewModel.ChatId = chatId;

            return PartialView("_MessagesPartialView", messageListViewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DisplayEmptyChat(string userId)
        {
            var getFullNameResult = await _userService.GetFullNameAsync(userId);
            if (!getFullNameResult.IsSuccess)
            {
                foreach (var error in getFullNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return View();
            }
            string? fullName = getFullNameResult.Value.FullName;

            MessageListViewModel messageListViewModel = new MessageListViewModel
            {
                FullName = fullName,
                PartnerId = userId,
            };
            return PartialView("_MessagesPartialView", messageListViewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMessage(CreateMessageViewModel createMessageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            string userId = GetUserId()!;

            var dto = _mapper.Map<CreateMessageDto>(createMessageViewModel);
            dto.UserId = userId;
            var createMessageResult = await _messageService.CreateMessageAsync(dto);
            if (!createMessageResult.IsSuccess)
            {
                if (!createMessageResult.IsSuccess)
                {
                    foreach (var error in createMessageResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Message);
                    }

                    return View();
                }
            }

            var getFullNameResult = await _userService.GetFullNameAsync(createMessageViewModel.PartnerId);
            if (!getFullNameResult.IsSuccess)
            {
                foreach (var error in getFullNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return View();
            }

            return Ok(new { createMessageResult.Value.ChatId, fullName = getFullNameResult.Value.FullName });
        }
    }
}
