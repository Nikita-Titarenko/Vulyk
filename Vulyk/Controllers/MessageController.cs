using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Entities;
using Vulyk.Services;
using Vulyk.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace Vulyk.Controllers
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

        [Authorize]
        public async Task<IActionResult> Index(int chatId, string partnerUserId)
        {
            string userId = GetUserId()!;

            MessageListDto messageListDto = await _messageService.GetMessagesAsync(chatId, userId, partnerUserId);
            MessageListViewModel messageListViewModel = _mapper.Map<MessageListViewModel>(messageListDto);
            messageListViewModel.ChatId = chatId;

            return PartialView("_MessagesPartialView", messageListViewModel);
        }

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
                UserId = userId,
            };
            return PartialView("_MessagesPartialView", messageListViewModel);
        }

        [Authorize]
        public async Task<IActionResult> CreateMessage(CreateMessageViewModel createMessageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            string userId = GetUserId()!;

            int chatId = await _messageService.CreateMessageAsync(userId, createMessageViewModel.Text, createMessageViewModel.UserId);

            var getFullNameResult = await _userService.GetFullNameAsync(createMessageViewModel.UserId);
            if (!getFullNameResult.IsSuccess)
            {
                foreach (var error in getFullNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }

                return View();
            }

            return Ok(new {chatId, fullName = getFullNameResult.Value.FullName });
        }
    }
}
