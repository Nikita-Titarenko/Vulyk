using System.ComponentModel;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Vulyk.Data;
using Vulyk.DTOs;
using Vulyk.Hubs;
using Vulyk.Models;

namespace Vulyk.Services
{
    public class MessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ChatService _chatService;
        private readonly ChatPartnerService _chatPartnerService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageService(ApplicationDbContext context, ChatPartnerService chatPartnerService, ChatService chatService, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _chatPartnerService = chatPartnerService;
            _chatService = chatService;
            _hubContext = hubContext;
        }

        public async Task<MessageListDto> GetMessagesAsync(int chatId, int userId)
        {
            var partner = await _chatPartnerService.GetChatPartnerAsync(userId, chatId);
            if (partner == null)
            {
                throw new InvalidOperationException("User not found");
            }
            MessageListDto messageListDto = new MessageListDto
            {
                ChatId = chatId,
                UserId = partner.Id,
                UserName = partner.Name,
                Messages = await _context.Message
                .Where(m => m.ChatId == chatId)
                .Select(m => new MessageListItemDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    CreationDateTime = m.CreationDateTime,
                    Text = m.Text,
                    IsMine = m.UserId == userId
                }).ToListAsync()
            };
            return messageListDto;
        }

        public async Task<int> CreateOrAddMessageToChat(int userId, string text, int userToAddId)
        {
            var result = await _chatService.GetOrCreateChatAsync(userId, userToAddId);
            if (result.Item1 == ChatService.CreateChatResult.Success)
            {
                int chatId = result.Item2.Value;
                _context.Message.Add(new Message
                {
                    UserId = userId,
                    ChatId = chatId,
                    Text = text,
                    CreationDateTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return result.Item2.Value;
        }
    }
}
