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

        public async Task<IActionResult> Index(int chatId)
        {
            int? userId = GetUserIdFromCookie();
            if (userId == null)
            {
                ShowUnexpectedError();
            }
            MessageListDto messageListDto = await _messageService.GetMessagesAsync(chatId, userId.Value);
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
            
            MessageListViewModel messageListViewModel = new MessageListViewModel
            {
                UserName = await _userService.GetUserNameAsync(userId),
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
            int chatId = await _messageService.CreateOrAddMessageToChat(userId.Value, createMessageViewModel.Text, createMessageViewModel.UserId);
            return Ok(chatId);
        }
    }
}
