using Microsoft.AspNetCore.Mvc;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Models;
using Vulyk.Services;
using Vulyk.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace Vulyk.Controllers
{
    public class MessageController : BaseController
    {
        private readonly MessageService _messageService;

        private readonly UserService _userService;

        public MessageController(MessageService messageService, UserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int chatId, int partnerUserId)
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            MessageListDto messageListDto = await _messageService.GetMessagesAsync(chatId, userId!.Value, partnerUserId);
            MessageListViewModel messageListViewModel = new MessageListViewModel
            {
                UserId = messageListDto.UserId,
                ChatId = chatId,
                UserName = messageListDto.UserName,
                Messages = messageListDto.Messages.Select(m => new MessageListItemViewModel
                {
                    CreationDateTime = m.CreationDateTime,
                    Id = m.Id,
                    Text = m.Text,
                    IsMine = m.IsMine,
                }).ToList()
            };

            return PartialView("_MessagesPartialView", messageListViewModel);
        }

        public async Task<IActionResult> DisplayEmptyChat(int userId)
        {
            string? userName = await _userService.GetUserNameAsync(userId);
            if (userName == null)
            {
                return BadRequest();
            }
            MessageListViewModel messageListViewModel = new MessageListViewModel
            {
                UserName = userName,
                UserId = userId,
            };
            return PartialView("_MessagesPartialView", messageListViewModel);
        }

        public async Task<IActionResult> CreateMessage(CreateMessageViewModel createMessageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                return ShowUnexpectedError();
            }
            int chatId = await _messageService.CreateOrAddMessageToChatAsync(userId.Value, createMessageViewModel.Text, createMessageViewModel.UserId);
            string? name = await _userService.GetUserNameAsync(createMessageViewModel.UserId);
            if (name == null)
            {
                return NotFound("User not found");
            }
            return Ok(new {chatId, name});
        }
    }
}
