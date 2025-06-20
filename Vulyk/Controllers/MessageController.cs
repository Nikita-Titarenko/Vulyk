using Microsoft.AspNetCore.Mvc;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Services;
using Vulyk.ViewModels;

namespace Vulyk.Controllers
{
    public class MessageController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int chatId)
        {
            MessageService messageService = new MessageService(_context);
            MessageListDto messageListDto = await messageService.GetMessages(chatId);
            MessageListViewModel messageListViewModel = new MessageListViewModel
            {
                ChatId = chatId,
                UserName = messageListDto.UserName,
                Messages = messageListDto.Messages.Select(m => new MessageListItemViewModel
                {
                    CreationDateTime = m.CreationDateTime,
                    Id = m.Id,
                    Text = m.Text,
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
            MessageService messageService = new MessageService(_context);
            await messageService.CreateMessage(createMessageViewModel.ChatId, userId.Value, createMessageViewModel.Text);
            return Ok();
        }
    }
}
