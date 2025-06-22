using Microsoft.AspNetCore.Mvc;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Services;
using Vulyk.ViewModels;

namespace Vulyk.Controllers
{
    public class MessageController : BaseController
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
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
            await _messageService.CreateMessage(createMessageViewModel.ChatId, userId.Value, createMessageViewModel.Text);
            return Ok();
        }
    }
}
